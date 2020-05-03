using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Audio;
using WPF_Soundboard.Audio.Clips.ClipPlayerHandlers;
using WPF_Soundboard.Hotkeys;

namespace WPF_Soundboard.Clips
{
    public class Clip
    {
        public event EventHandler OnChanged;

        [JsonProperty]
        private GuiInfo guiInfo;

        [JsonProperty]
        private SoundInfo soundInfo;

        [JsonProperty]
        private HotkeyInfo hotkeyInfo;

        [JsonIgnore]
        private Hotkey Hotkey = null;

        [JsonIgnore]
        private IClipPlayerHandler playerHandler;

        public Guid Guid { get; }
        public GuiInfo GuiInfo { get => guiInfo; set { guiInfo = value; CallOnChanged(); } }
        public SoundInfo SoundInfo
        {
            get => soundInfo; set
            {
                soundInfo = value;
                CallOnChanged();
                playerHandler?.Stop();
                if (soundInfo.Enabled && File.Exists(soundInfo.Path))
                    playerHandler = GetClipPlayerHandler(soundInfo);
            }
        }
        public HotkeyInfo HotkeyInfo { get => hotkeyInfo; set { hotkeyInfo = value; CallOnChanged(); InitializeHotkey(); } }



        public Clip(Guid? guid = null, GuiInfo guiInfo = default, SoundInfo soundInfo = default, HotkeyInfo hotkeyInfo = default)
        {
            Guid = guid ?? Guid.NewGuid();
            this.guiInfo = guiInfo;
            this.soundInfo = soundInfo;
            this.hotkeyInfo = hotkeyInfo;

            if (soundInfo.Enabled && File.Exists(soundInfo.Path))
                playerHandler = GetClipPlayerHandler(soundInfo);

            InitializeHotkey();
        }

        private void InitializeHotkey()
        {
            if (Hotkey != null)
            {
                Hotkey.Dispose();
                Hotkey = null;
            }

            if (HotkeyInfo.Enabled)
            {
                Hotkey = new Hotkey(HotkeyInfo.ModifierKeys, HotkeyInfo.Key);
                Hotkey.Register();
                Hotkey.Pressed += Hotkey_Pressed;
            }
        }

        private void Hotkey_Pressed(object sender, EventArgs e) => Play();

        public void Play() => playerHandler?.Play();

        private void CallOnChanged() => OnChanged?.Invoke(this, null);

        private static IClipPlayerHandler GetClipPlayerHandler(SoundInfo info) => info.Behavior switch
        {
            SoundInfo.PlayBehavior.Restart => new RestartingClipPlayerHandler(info),
            SoundInfo.PlayBehavior.StartNew => new MultiFireClipPlayerHandler(info),
            SoundInfo.PlayBehavior.Stop => new StoppingClipPlayerHandler(info),
            _ => throw new NotImplementedException(),
        };

    }
}
