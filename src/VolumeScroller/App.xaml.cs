using System;
using System.Threading;
using System.Windows;
using Forms = System.Windows.Forms;
using VolumeScroller.Audio;
using VolumeScroller.Input;
using VolumeScroller.Tray;
using VolumeScroller.UI;

namespace VolumeScroller
{
    public partial class App : System.Windows.Application
    {
        private const string MutexName = "VolumeScroller_SingleInstance_Mutex";
        private Mutex? _singleInstanceMutex;

        private AudioController? _audioController;
        private OsdWindow? _osdWindow;
        private InputHook? _inputHook;
        private TrayIconManager? _trayIconManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            _singleInstanceMutex = new Mutex(true, MutexName, out bool isNewInstance);
            if (!isNewInstance)
            {
                Forms.MessageBox.Show(
                    "Volume Scroller est déjà en cours d'exécution dans la barre des tâches.",
                    "Volume Scroller",
                    Forms.MessageBoxButtons.OK,
                    Forms.MessageBoxIcon.Information);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            try
            {
                _audioController = new AudioController();
                _osdWindow = new OsdWindow();

                _inputHook = new InputHook();
                _inputHook.OnVolumeWheel += HandleVolumeWheel;
                _inputHook.Start();

                _trayIconManager = new TrayIconManager(_audioController, _osdWindow);
            }
            catch (Exception ex)
            {
                Forms.MessageBox.Show(
                    $"Une erreur est survenue lors du démarrage de Volume Scroller :\n{ex.Message}",
                    "Erreur - Volume Scroller",
                    Forms.MessageBoxButtons.OK,
                    Forms.MessageBoxIcon.Error);
                Shutdown();
            }
        }

        private void HandleVolumeWheel(int stepPercent)
        {
            if (_audioController == null || _osdWindow == null) return;

            var (newVol, isMuted) = _audioController.AdjustVolume(stepPercent);
            _osdWindow.ShowVolume(newVol, isMuted);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _inputHook?.Dispose();
            _trayIconManager?.Dispose();
            _osdWindow?.Close();

            if (_singleInstanceMutex != null)
            {
                try
                {
                    _singleInstanceMutex.ReleaseMutex();
                }
                catch { }
                _singleInstanceMutex.Dispose();
            }

            base.OnExit(e);
        }
    }
}
