using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_Soundboard.Hotkeys;

namespace WPF_Soundboard.Clips
{
    public struct HotkeyInfo
    {
        public bool Enabled { get; set; }

        public ModifierKeys ModifierKeys { get; set; }
        public Keys Key { get; set; }

        public override string ToString()
        {
            if (!Enabled)
                return "";

            if (ModifierKeys == ModifierKeys.None)
            {
                return Key.ToString();
            }
            return ModifierKeys.ToString().Replace(", ", " + ").Replace("Control", "Ctrl") + " + " + Key.ToString();
        }
    }
}
