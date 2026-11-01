using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Forms = System.Windows.Forms;
using VolumeScroller.Config;

namespace VolumeScroller.Input
{
    public class InputHook : IDisposable
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEWHEEL = 0x020A;

        public delegate void VolumeWheelAction(int stepPercent);
        public event VolumeWheelAction? OnVolumeWheel;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly LowLevelMouseProc _mouseProc;
        private IntPtr _mouseHookId = IntPtr.Zero;
        private readonly AppSettings _settings;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT pt);

        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        public InputHook(AppSettings settings)
        {
            _settings = settings;
            _mouseProc = MouseHookCallback;
        }

        public void Start()
        {
            if (_mouseHookId == IntPtr.Zero)
            {
                using var curProcess = Process.GetCurrentProcess();
                using var curModule = curProcess.MainModule;
                IntPtr hMod = GetModuleHandle(curModule?.ModuleName);
                _mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, hMod, 0);
            }
        }

        public void Stop()
        {
            if (_mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookId);
                _mouseHookId = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL)
            {
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                short rawDelta = (short)((hookStruct.mouseData >> 16) & 0xffff);

                // Trigger 1: Configured shortcut key combination held down
                bool isShortcutDown = false;
                var keys = _settings.ShortcutKeys;
                if (keys != null && keys.Count > 0)
                {
                    isShortcutDown = true;
                    foreach (int vk in keys)
                    {
                        if ((GetAsyncKeyState(vk) & 0x8000) == 0)
                        {
                            isShortcutDown = false;
                            break;
                        }
                    }
                }

                // Trigger 2: Cursor over taskbar area
                bool isOverTaskbar = IsPointOverTaskbar(hookStruct.pt);

                if (isShortcutDown || isOverTaskbar)
                {
                    int notches = rawDelta / 120;
                    if (notches == 0)
                    {
                        notches = rawDelta > 0 ? 1 : -1;
                    }
                    int stepPercent = notches * 2;

                    OnVolumeWheel?.Invoke(stepPercent);

                    // Block wheel scroll so underlying document / taskbar doesn't scroll
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }

        private static bool IsPointOverTaskbar(POINT pt)
        {
            try
            {
                var screenPoint = new System.Drawing.Point(pt.x, pt.y);
                var screen = Forms.Screen.FromPoint(screenPoint);

                // If point is within screen bounds but outside working area, it's the taskbar!
                if (screen.Bounds.Contains(screenPoint) && !screen.WorkingArea.Contains(screenPoint))
                {
                    return true;
                }

                // Fallback check on window class
                IntPtr hwnd = WindowFromPoint(pt);
                if (hwnd != IntPtr.Zero)
                {
                    if (IsTaskbarWindow(hwnd)) return true;
                    IntPtr root = GetAncestor(hwnd, 2); // GA_ROOT
                    if (root != IntPtr.Zero && IsTaskbarWindow(root)) return true;
                }
            }
            catch { }

            return false;
        }

        private static bool IsTaskbarWindow(IntPtr hwnd)
        {
            var sb = new StringBuilder(256);
            if (GetClassName(hwnd, sb, sb.Capacity) > 0)
            {
                string name = sb.ToString();
                if (name == "Shell_TrayWnd" || name == "Shell_SecondaryTrayWnd")
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
