using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WPF_Soundboard.Hotkeys;

namespace WPF_Soundboard.Clips
{
    public class GlobalHotkey : IDisposable
    {
        private Hotkey hotkey;
        [JsonIgnore]
        public bool IsEnabled => GlobalHotkeyInfo.HotkeyInfo.Enabled;

        private GlobalHotkeyInfo _globalHotkeyInfo;
        [JsonProperty]
        public GlobalHotkeyInfo GlobalHotkeyInfo
        {
            get => _globalHotkeyInfo;
            set
            {
                _globalHotkeyInfo = value;
                InitializeHotKey();
                CallOnChanged();
            }
        }

        public event EventHandler OnChanged;
        public event EventHandler<GlobalHotkeyEventArgs> OnHotkeyPressed;

        [JsonConstructor]
        public GlobalHotkey(GlobalHotkeyInfo globalHotkeyInfo)
        {
            _globalHotkeyInfo = globalHotkeyInfo;
            InitializeHotKey();
        }

        private void InitializeHotKey()
        {
            if (GlobalHotkeyInfo.HotkeyInfo.Enabled)
            {
                hotkey = new Hotkey(GlobalHotkeyInfo.HotkeyInfo.ModifierKeys, GlobalHotkeyInfo.HotkeyInfo.Key);
                hotkey.Pressed += Hotkey_Pressed;
                hotkey.Register();
            }
        }

        private void Hotkey_Pressed(object sender, EventArgs e) => OnHotkeyPressed?.Invoke(this, new GlobalHotkeyEventArgs() { X = GlobalHotkeyInfo.x, Y = GlobalHotkeyInfo.y });
        private void CallOnChanged() => OnChanged?.Invoke(this, EventArgs.Empty);
#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose() => ((IDisposable)hotkey).Dispose();
#pragma warning restore CA1063 // Implement IDisposable Correctly
    }
}
