<div align="center">
  <img src="VolumeScroller-icon.png" alt="Volume Scroller Logo" height="180" />
  
  <h1><font size="7">Volume Scroller</font></h1>
  
  <h3>Fast, native, and lightweight Windows volume controller at your fingertips.</h3>
  
  <p>
    Adjust system master volume effortlessly with your mouse wheel — anywhere on your screen or directly over the taskbar.
  </p>

  <br/>

  <p>
    <a href="https://github.com"><img src="https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011-0078D6?logo=windows&logoColor=white" alt="Platform Windows" height="20" /></a>
    <a href="https://dotnet.microsoft.com"><img src="https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 9" height="20" /></a>
    <a href="#"><img src="https://img.shields.io/badge/UI-Modern%20Floating%20OSD-2d3748" alt="UI" height="20" /></a>
    <a href="#"><img src="https://img.shields.io/badge/Architecture-Zero%20Electron-success" alt="Zero Electron" height="20" /></a>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License MIT" height="20" /></a>
    <a href="../../releases/latest"><img src="https://img.shields.io/badge/Version-1.0.0-blue" alt="Latest Version" height="20" /></a>
  </p>
  
  <p>
    🇫🇷 <em><a href="README-fr.md">Documentation en français disponible ici</a></em>
  </p>
</div>

> [!NOTE]
> ### ☕ A quick note from the creator
> **Full disclosure:** I am not a software developer, and GitHub / open-source culture is completely new territory for me! 
> 
> I **vibe-coded** Volume Scroller simply because I wanted a clean, frictionless way to fine-tune my volume on Windows without reaching for keyboard media keys, opening system menus, or dealing with heavy bloated background apps.
> 
> If you submit Issues or Pull Requests, **please be patient and gentle with me**. I’ll do my best to review things when I can, but software development is not my day job. Thank you so much for your understanding, kindness, and support! ❤️

---

## 💡 Why Volume Scroller?

Controlling audio volume on Windows often feels clunky: you either have to click the tiny speaker icon in the taskbar, reach for keyboard function keys, or install heavy, bloated customization software that eats memory and conflicts with existing shortcuts.

**Volume Scroller** is designed with a simple philosophy: **instant, intuitive control with zero bloat.**
* **Ctrl + Alt + Scroll** (or your own customizable shortcut) anywhere on your desktop or inside any app.
* **Scroll over the Taskbar** with a quick flick of your mouse, single-handed without touching the keyboard.
* A sleek floating on-screen pill follows your cursor and guarantees full visibility without stealing window focus.

---

## ⚡ Key Features

* **🚀 100% Native Windows & Zero Web Bloat**
  * Built with **C# / .NET 9 + Win32 CoreAudio APIs**.
  * Zero Electron, zero Chromium, zero webviews, zero background telemetries.
  * Ultra-compact standalone executable (~195 KB) with near-zero CPU and ~15 MB RAM usage.

* **🖱️ Natural Dual-Trigger Controls & Custom Shortcut**
  * **`Ctrl` + `Alt` + Mouse Wheel (Customizable)**: Adjust volume instantly wherever your cursor is (games, full-screen videos, creative apps) with zero conflict in editing software (DaVinci Resolve, Premiere Pro...). Easily rebind your trigger anytime via the tray menu!
  * **Mouse Wheel over Taskbar**: Hover over any taskbar region (primary or secondary monitors) and scroll without touching the keyboard.
  * **Scroll interception**: Page or document scrolling under the mouse is automatically suppressed while adjusting volume.

* **🎯 Smart Cursor-Tracking OSD Pill**
  * Renders a modern, semi-transparent pill right at your cursor position with real-time volume percentage (`XX%`) and dynamic speaker states.
  * **Anti-Clipping Edge Guard**: If your cursor is near the edges or corners of any screen, the pill automatically stays within visible bounds and never clips off-screen.
  * **Always-on-Top (`HWND_TOPMOST`)**: Stays in the absolute foreground over games and full-screen windows.
  * **Non-intrusive**: Click-through (`IsHitTestVisible="False"`) and never steals focus from your active apps (`SWP_NOACTIVATE`).

* **🔊 Direct Windows CoreAudio Integration**
  * Talks directly to Windows native COM interfaces (`IAudioEndpointVolume`).
  * Instant response with zero latency.
  * **Auto-unmute**: Automatically unmutes audio when scrolling up if muted.

* **⚙️ Effortless "Start with Windows" Management**
  * **At installation**: Select the checkbox during setup to launch automatically at Windows boot.
  * **After installation**: Right-click the system tray icon to toggle **"Démarrer avec Windows" / "Start with Windows"** on or off in real time without digging through system settings.
  * Clean registry management in `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.

---

## 📦 Download & Installation

Head over to the **[GitHub Releases](../../releases/latest)** page to grab the latest build:

| Package | Description | How to Run |
|---|---|---|
| **`VolumeScroller-Setup.exe`** | Official Windows installer (Inno Setup) | Run setup. Supports "Start with Windows" choice and clean uninstaller. |
| **`VolumeScroller.exe`** | Standalone portable executable (~195 KB) | Double-click to launch immediately without installation. |
| **`VolumeScroller-Portable.zip`** | Portable ZIP archive | Unpack anywhere and run. |

---

## 🎮 Shortcuts & Controls

| Shortcut / Action | Function |
|---|---|
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Wheel Up</kbd> | Increase volume (+2%) *(rebindable)* |
| <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>Wheel Down</kbd> | Decrease volume (-2%) *(rebindable)* |
| <kbd>Wheel</kbd> over Taskbar | Adjust volume without keyboard |
| **"Modifier le raccourci global..."** | Right-click tray icon > press your keys and validate |
| **Double-Click Systray Icon** | Preview / test the OSD floating pill |
| **Right-Click Systray Icon** | Open context menu (Edit shortcut, Toggle Startup, Test OSD, Quit) |

---

## 📁 Project Structure

```text
VolumeScroller/
├── installer/
│   └── VolumeScroller.iss           # Inno Setup compilation script
├── publish/
│   ├── Installer/
│   │   └── VolumeScroller-Setup.exe # Official ready-to-run installer
│   ├── VolumeScroller-Portable/
│   │   ├── VolumeScroller.exe       # Portable single-file executable
│   │   └── icon.ico
│   └── VolumeScroller-Portable.zip  # Compressed portable archive
├── src/
│   └── VolumeScroller/              # C# / .NET 9 source code
│       ├── Audio/                   # Windows CoreAudio COM interfaces
│       ├── Input/                   # Low-level mouse hook & taskbar detection
│       ├── Startup/                 # Windows Registry Run key manager
│       ├── Tray/                    # System tray icon & context menu
│       └── UI/                      # Floating OSD pill (WPF)
├── build.ps1                        # 1-Click build & packaging script
├── icon.ico                         # Main application icon
├── VolumeScroller-icon.png          # High-resolution logo for documentation
├── README.md                        # English documentation
└── README-fr.md                     # French documentation
```

---

## 🛠️ Building from Source

### Prerequisites
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
* *(Optional, for installer generation)* [Inno Setup 6](https://jrsoftware.org/isdl.php)

### 1-Click Build (PowerShell)
```powershell
.\build.ps1
```
This script will:
1. Compile the Release single-file executable into `publish/VolumeScroller-Portable/`.
2. Generate the portable ZIP package.
3. Automatically invoke Inno Setup to produce `publish/Installer/VolumeScroller-Setup.exe`.

---

## 📄 License

This project is open-source under the [MIT License](LICENSE).
