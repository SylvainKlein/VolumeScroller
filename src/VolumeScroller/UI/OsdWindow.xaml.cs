using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Forms = System.Windows.Forms;
using MediaColor = System.Windows.Media.Color;
using MediaBrushes = System.Windows.Media.Brushes;

namespace VolumeScroller.UI
{
    public partial class OsdWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private readonly DispatcherTimer _hideTimer;
        private readonly DoubleAnimation _fadeOutAnimation;

        public OsdWindow()
        {
            InitializeComponent();

            _hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1100)
            };
            _hideTimer.Tick += OnHideTimerTick;

            _fadeOutAnimation = new DoubleAnimation
            {
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            _fadeOutAnimation.Completed += (s, e) =>
            {
                Visibility = Visibility.Hidden;
            };

            // Pre-create HWND to apply tool window styles
            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();

            int exStyle = GetWindowLong(helper.Handle, GWL_EXSTYLE);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
        }

        public void ShowVolume(int volumePercent, bool isMuted)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Reset hide timer and stop any ongoing fade
                _hideTimer.Stop();
                RootGrid.BeginAnimation(UIElement.OpacityProperty, null);
                RootGrid.Opacity = 1.0;

                // Update contents
                if (isMuted)
                {
                    VolumeText.Text = "Mute";
                    VolumeText.Foreground = new SolidColorBrush(MediaColor.FromRgb(0xFF, 0x55, 0x55));
                    MuteCrossPath.Visibility = Visibility.Visible;
                    WaveLowPath.Visibility = Visibility.Collapsed;
                    WaveHighPath.Visibility = Visibility.Collapsed;
                    VolumeTrackFill.Width = 0;
                    VolumeTrackFill.Background = new SolidColorBrush(MediaColor.FromRgb(0x88, 0x88, 0x88));
                }
                else
                {
                    VolumeText.Text = $"{volumePercent}%";
                    VolumeText.Foreground = MediaBrushes.White;
                    MuteCrossPath.Visibility = Visibility.Collapsed;
                    WaveLowPath.Visibility = volumePercent > 0 ? Visibility.Visible : Visibility.Collapsed;
                    WaveHighPath.Visibility = volumePercent > 50 ? Visibility.Visible : Visibility.Collapsed;

                    double maxTrackWidth = 110.0;
                    VolumeTrackFill.Width = Math.Round((volumePercent / 100.0) * maxTrackWidth);
                    VolumeTrackFill.Background = new SolidColorBrush(MediaColor.FromRgb(0x00, 0x78, 0xD4));
                }

                // Make visible so WPF can compute layout
                Visibility = Visibility.Visible;
                UpdateLayout();

                // Compute position based on cursor and actual size
                var cursorPos = Forms.Cursor.Position;
                var screen = Forms.Screen.FromPoint(cursorPos);
                var area = screen.WorkingArea;

                var helper = new WindowInteropHelper(this);
                var source = PresentationSource.FromVisual(this);
                double dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
                double dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

                double winWidth = ActualWidth > 0 ? ActualWidth : 280;
                double winHeight = ActualHeight > 0 ? ActualHeight : 66;

                double physicalW = winWidth * dpiX;
                double physicalH = winHeight * dpiY;

                // Position above cursor
                int posX = (int)(cursorPos.X - physicalW / 2.0);
                int posY = (int)(cursorPos.Y - physicalH - (14 * dpiY));

                // Flip below cursor if near top
                if (posY < area.Top + (8 * dpiY))
                {
                    posY = (int)(cursorPos.Y + (22 * dpiY));
                }

                // Clamp strictly inside monitor visible area
                int marginX = (int)(8 * dpiX);
                int marginY = (int)(8 * dpiY);
                posX = Math.Clamp(posX, area.Left + marginX, area.Right - (int)physicalW - marginX);
                posY = Math.Clamp(posY, area.Top + marginY, area.Bottom - (int)physicalH - marginY);

                // Update WPF DIP coordinates
                Left = posX / dpiX;
                Top = posY / dpiY;

                // Move and show window with topmost Z-order WITHOUT resizing (SWP_NOSIZE)
                SetWindowPos(helper.Handle, HWND_TOPMOST, posX, posY, 0, 0, SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);

                // Start hide countdown
                _hideTimer.Start();
            }));
        }

        private void OnHideTimerTick(object? sender, EventArgs e)
        {
            _hideTimer.Stop();
            RootGrid.BeginAnimation(UIElement.OpacityProperty, _fadeOutAnimation);
        }
    }
}
