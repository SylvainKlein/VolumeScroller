using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Forms = System.Windows.Forms;
using VolumeScroller.Audio;
using VolumeScroller.Startup;
using VolumeScroller.UI;

namespace VolumeScroller.Tray
{
    public class TrayIconManager : IDisposable
    {
        private Forms.NotifyIcon? _notifyIcon;
        private readonly AudioController _audioController;
        private readonly OsdWindow _osdWindow;
        private Forms.ToolStripMenuItem? _startupMenuItem;

        public TrayIconManager(AudioController audioController, OsdWindow osdWindow)
        {
            _audioController = audioController;
            _osdWindow = osdWindow;

            InitializeTray();
        }

        private void InitializeTray()
        {
            _notifyIcon = new Forms.NotifyIcon
            {
                Text = "Volume Scroller\nAlt + Molette ou Barre des tâches",
                Visible = true
            };

            // Load icon
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                if (File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                }
                else
                {
                    _notifyIcon.Icon = SystemIcons.Application;
                }
            }
            catch
            {
                _notifyIcon.Icon = SystemIcons.Application;
            }

            // Create context menu
            var contextMenu = new Forms.ContextMenuStrip();

            var titleItem = new Forms.ToolStripMenuItem("Volume Scroller v1.0")
            {
                Enabled = false,
                Font = new Font(contextMenu.Font, System.Drawing.FontStyle.Bold)
            };
            contextMenu.Items.Add(titleItem);
            contextMenu.Items.Add(new Forms.ToolStripSeparator());

            // Startup option (Run with Windows)
            _startupMenuItem = new Forms.ToolStripMenuItem("Démarrer avec Windows")
            {
                Checked = StartupManager.IsRunAtStartupEnabled()
            };
            _startupMenuItem.Click += (s, e) => ToggleStartup();
            contextMenu.Items.Add(_startupMenuItem);

            // Test OSD option
            var testOsdItem = new Forms.ToolStripMenuItem("Tester l'indicateur (OSD)");
            testOsdItem.Click += (s, e) =>
            {
                var (vol, muted) = _audioController.GetCurrentState();
                _osdWindow.ShowVolume(vol, muted);
            };
            contextMenu.Items.Add(testOsdItem);

            contextMenu.Items.Add(new Forms.ToolStripSeparator());

            // Exit option
            var exitItem = new Forms.ToolStripMenuItem("Quitter");
            exitItem.Click += (s, e) =>
            {
                System.Windows.Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Double click opens test OSD
            _notifyIcon.DoubleClick += (s, e) =>
            {
                var (vol, muted) = _audioController.GetCurrentState();
                _osdWindow.ShowVolume(vol, muted);
            };
        }

        private void ToggleStartup()
        {
            if (_startupMenuItem == null) return;

            bool currentStatus = StartupManager.IsRunAtStartupEnabled();
            bool newStatus = !currentStatus;

            bool success = StartupManager.SetRunAtStartup(newStatus);
            if (success)
            {
                _startupMenuItem.Checked = newStatus;
            }
            else
            {
                Forms.MessageBox.Show(
                    "Impossible de modifier l'option de démarrage automatique dans le registre.",
                    "Volume Scroller",
                    Forms.MessageBoxButtons.OK,
                    Forms.MessageBoxIcon.Warning);
            }
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
