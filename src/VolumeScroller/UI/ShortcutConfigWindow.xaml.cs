using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using VolumeScroller.Config;

namespace VolumeScroller.UI
{
    public partial class ShortcutConfigWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private readonly AppSettings _settings;
        private List<int> _pendingKeys;
        private string _pendingDisplayText;

        public event Action<AppSettings>? OnShortcutUpdated;

        public ShortcutConfigWindow(AppSettings settings)
        {
            InitializeComponent();

            _settings = settings;
            _pendingKeys = new List<int>(settings.ShortcutKeys);
            _pendingDisplayText = settings.ShortcutDisplayText;

            ShortcutText.Text = _pendingDisplayText;

            Loaded += (s, e) =>
            {
                Activate();
                Focus();
            };

            MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            };

            PreviewKeyDown += HandlePreviewKeyDown;
        }

        private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key realKey = e.Key == Key.System ? e.SystemKey : e.Key;

            // Escape cancels
            if (realKey == Key.Escape)
            {
                e.Handled = true;
                Close();
                return;
            }

            // Enter validates
            if (realKey == Key.Enter)
            {
                e.Handled = true;
                ValidateAndSave();
                return;
            }

            e.Handled = true;

            var keys = new List<int>();
            var labels = new List<string>();

            // Check modifiers state
            bool isCtrl = (GetAsyncKeyState(0x11) & 0x8000) != 0;
            bool isAlt = (GetAsyncKeyState(0x12) & 0x8000) != 0;
            bool isShift = (GetAsyncKeyState(0x10) & 0x8000) != 0;
            bool isWin = (GetAsyncKeyState(0x5B) & 0x8000) != 0 || (GetAsyncKeyState(0x5C) & 0x8000) != 0;

            if (isCtrl)
            {
                keys.Add(0x11);
                labels.Add("Ctrl");
            }
            if (isAlt)
            {
                keys.Add(0x12);
                labels.Add("Alt");
            }
            if (isShift)
            {
                keys.Add(0x10);
                labels.Add("Shift");
            }
            if (isWin)
            {
                keys.Add(0x5B);
                labels.Add("Win");
            }

            int vk = KeyInterop.VirtualKeyFromKey(realKey);

            // If the triggered key is NOT a modifier, add it as a primary key
            if (!IsModifierVk(vk))
            {
                keys.Add(vk);
                labels.Add(FormatKeyName(realKey, vk));
            }

            if (keys.Count > 0)
            {
                _pendingKeys = keys;
                _pendingDisplayText = string.Join(" + ", labels);
                ShortcutText.Text = _pendingDisplayText;
            }
        }

        private static bool IsModifierVk(int vk)
        {
            return vk switch
            {
                0x10 or 0x11 or 0x12 or 0x5B or 0x5C => true, // Shift, Ctrl, Alt, Win
                0xA0 or 0xA1 or 0xA2 or 0xA3 or 0xA4 or 0xA5 => true, // L/R variants
                _ => false
            };
        }

        private static string FormatKeyName(Key key, int vk)
        {
            return key switch
            {
                Key.Space => "Espace",
                Key.Tab => "Tab",
                Key.CapsLock => "Caps Lock",
                Key.OemTilde or Key.Oem3 => "² / ~",
                Key.OemQuotes => "²",
                Key.OemPlus => "+",
                Key.OemMinus => "-",
                Key.OemComma => ",",
                Key.OemPeriod => ".",
                _ when key >= Key.A && key <= Key.Z => key.ToString(),
                _ when key >= Key.D0 && key <= Key.D9 => key.ToString().TrimStart('D'),
                _ when key >= Key.NumPad0 && key <= Key.NumPad9 => "Num " + key.ToString()[6..],
                _ when key >= Key.F1 && key <= Key.F24 => key.ToString(),
                _ => key.ToString()
            };
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndSave();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ValidateAndSave()
        {
            if (_pendingKeys.Count > 0)
            {
                _settings.ShortcutKeys = new List<int>(_pendingKeys);
                _settings.ShortcutDisplayText = _pendingDisplayText;
                SettingsManager.Save(_settings);
                OnShortcutUpdated?.Invoke(_settings);
            }
            Close();
        }
    }
}
