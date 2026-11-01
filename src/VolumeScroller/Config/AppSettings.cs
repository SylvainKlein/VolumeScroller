using System.Collections.Generic;

namespace VolumeScroller.Config
{
    public class AppSettings
    {
        // 0x11 = VK_CONTROL, 0x12 = VK_MENU (Alt) -> Ctrl + Alt by default
        public List<int> ShortcutKeys { get; set; } = new() { 0x11, 0x12 };
        public string ShortcutDisplayText { get; set; } = "Ctrl + Alt";
    }
}
