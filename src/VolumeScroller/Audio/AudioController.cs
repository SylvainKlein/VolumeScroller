using System;
using System.Runtime.InteropServices;

namespace VolumeScroller.Audio
{
    public class AudioController
    {
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumeratorComObject { }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            [PreserveSig] int EnumAudioEndpoints(int dataFlow, uint dwStateMask, out IntPtr ppDevices);
            [PreserveSig] int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
            [PreserveSig] int OpenPropertyStore(int stgmAccess, out IntPtr ppProperties);
            [PreserveSig] int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);
            [PreserveSig] int GetState(out uint pdwState);
        }

        // Official Windows IAudioEndpointVolume interface GUID from endpointvolume.h
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioEndpointVolume
        {
            [PreserveSig] int RegisterControlChangeNotify(IntPtr pNotify);
            [PreserveSig] int UnregisterControlChangeNotify(IntPtr pNotify);
            [PreserveSig] int GetChannelCount(out uint pnChannelCount);
            [PreserveSig] int SetMasterVolumeLevel(float fLevelDB, ref Guid pguidEventContext);
            [PreserveSig] int SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);
            [PreserveSig] int GetMasterVolumeLevel(out float pfLevelDB);
            [PreserveSig] int GetMasterVolumeLevelScalar(out float pfLevel);
            [PreserveSig] int SetChannelVolumeLevel(uint nChannel, float fLevelDB, ref Guid pguidEventContext);
            [PreserveSig] int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, ref Guid pguidEventContext);
            [PreserveSig] int GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);
            [PreserveSig] int GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);
            [PreserveSig] int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid pguidEventContext);
            [PreserveSig] int GetMute([MarshalAs(UnmanagedType.Bool)] out bool pbMute);
            [PreserveSig] int GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);
            [PreserveSig] int VolumeStepUp(ref Guid pguidEventContext);
            [PreserveSig] int VolumeStepDown(ref Guid pguidEventContext);
            [PreserveSig] int QueryHardwareSupport(out uint pdwHardwareSupportMask);
            [PreserveSig] int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
        }

        private const int EDataFlow_Render = 0;
        private const int ERole_Multimedia = 1;
        private const int CLSCTX_ALL = 23;
        private static readonly Guid IID_IAudioEndpointVolume = new("5CDF2C82-841E-4546-9722-0CF74078229A");

        private IAudioEndpointVolume? GetEndpointVolume()
        {
            try
            {
                var enumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
                int hr = enumerator.GetDefaultAudioEndpoint(EDataFlow_Render, ERole_Multimedia, out var device);
                if (hr != 0 || device == null) return null;

                hr = device.Activate(IID_IAudioEndpointVolume, CLSCTX_ALL, IntPtr.Zero, out var endpointObj);
                if (hr != 0 || endpointObj == null) return null;

                return (IAudioEndpointVolume)endpointObj;
            }
            catch
            {
                return null;
            }
        }

        public (int volumePercent, bool isMuted) GetCurrentState()
        {
            var endpoint = GetEndpointVolume();
            if (endpoint == null) return (0, false);

            endpoint.GetMasterVolumeLevelScalar(out float level);
            endpoint.GetMute(out bool isMuted);

            int percent = (int)Math.Round(level * 100f);
            return (Math.Clamp(percent, 0, 100), isMuted);
        }

        public (int newVolumePercent, bool isMuted) AdjustVolume(int stepPercent)
        {
            var endpoint = GetEndpointVolume();
            if (endpoint == null) return (0, false);

            endpoint.GetMasterVolumeLevelScalar(out float currentLevel);
            endpoint.GetMute(out bool isMuted);

            int currentPercent = (int)Math.Round(currentLevel * 100f);
            int newPercent = Math.Clamp(currentPercent + stepPercent, 0, 100);

            Guid guid = Guid.Empty;

            // If increasing volume while muted, automatically unmute
            if (stepPercent > 0 && isMuted)
            {
                endpoint.SetMute(false, ref guid);
                isMuted = false;
            }

            float newScalar = newPercent / 100f;
            endpoint.SetMasterVolumeLevelScalar(newScalar, ref guid);

            return (newPercent, isMuted);
        }

        public (int volumePercent, bool isMuted) ToggleMute()
        {
            var endpoint = GetEndpointVolume();
            if (endpoint == null) return (0, false);

            endpoint.GetMute(out bool isMuted);
            bool newMuted = !isMuted;
            Guid guid = Guid.Empty;
            endpoint.SetMute(newMuted, ref guid);

            endpoint.GetMasterVolumeLevelScalar(out float level);
            int percent = (int)Math.Round(level * 100f);

            return (percent, newMuted);
        }
    }
}
