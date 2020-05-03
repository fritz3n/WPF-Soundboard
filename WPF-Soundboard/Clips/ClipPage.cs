using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Hotkeys;

namespace WPF_Soundboard.Clips
{
    public class ClipPage
    {
        private string name = "Page";

        public event EventHandler OnChanged;
        public event EventHandler OnHotkeyPressed;

        [JsonProperty]
        public Clip[][] Clips { get; private set; }
        [JsonProperty]
        public int Width { get; private set; }
        [JsonProperty]
        public int Height { get; private set; }


        [JsonIgnore]
        private Hotkey Hotkey = null;
        private HotkeyInfo hotkeyInfo;

        public string Name { get => name; set { name = value; CallOnChanged(); } }

        public HotkeyInfo HotkeyInfo { get => hotkeyInfo; set { hotkeyInfo = value; CallOnChanged(); InitializeHotkey(); } }

        public Clip this[int x, int y]
        {
            get => Clips[x][y];
            set { Clips[x][y] = value; value.OnChanged += Value_OnChanged; CallOnChanged(); }
        }

        private void Value_OnChanged(object sender, EventArgs e) => CallOnChanged();
        [JsonConstructor]
        private ClipPage(Clip[][] clips)
        {
            Clips = clips;
            foreach (Clip[] column in Clips)
            {
                foreach (Clip clip in column)
                {
                    if (clip != null)
                        clip.OnChanged += Value_OnChanged;
                }
            }
        }
        public ClipPage(HotkeyInfo hotkeyInfo = default)
        {
            this.hotkeyInfo = hotkeyInfo;
        }

        public ClipPage(int width, int height, HotkeyInfo hotkeyInfo = default) : this(hotkeyInfo)
        {
            Clips = new Clip[width][];

            for (int i = 0; i < Clips.Length; i++)
                Clips[i] = new Clip[height];
            Width = width;
            Height = height;
        }

        public ClipPage(IEnumerable<IEnumerable<Clip>> clips, int width, int height, HotkeyInfo hotkeyInfo = default) : this(width, height, hotkeyInfo)
        {
            int x = 0;
            int y = 0;

            foreach (IEnumerable<Clip> column in clips)
            {
                foreach (Clip clip in column)
                {
                    Clips[x][y++] = clip;
                }

                x++;
            }
        }

        private void InitializeHotkey()
        {
            if (Hotkey != null)
            {
                Hotkey.Dispose();
                Hotkey = null;
            }

            if (hotkeyInfo.Enabled)
            {
                Hotkey = new Hotkey(hotkeyInfo.ModifierKeys, hotkeyInfo.Key);
                Hotkey.Register();
                Hotkey.Pressed += Hotkey_Pressed;
            }
        }

        private void Hotkey_Pressed(object sender, EventArgs e) => OnHotkeyPressed?.Invoke(this, EventArgs.Empty);

        public void ChangeSize(int width, int height)
        {
            Clip[][] newClips = new Clip[width][];

            for (int i = 0; i < width; i++)
            {
                if (i < Width)
                {
                    if (height == Height)
                    {
                        newClips[i] = Clips[i];
                    }
                    else
                    {
                        Clip[] newColumn = new Clip[height];
                        Array.Copy(Clips[i], newColumn, Math.Min(height, Height));
                        newClips[i] = newColumn;
                    }
                }
                else
                {
                    newClips[i] = new Clip[height];
                }
            }

            Clips = newClips;
            Width = width;
            Height = height;
            CallOnChanged();
        }


        private void CallOnChanged() => OnChanged?.Invoke(this, null);
    }
}
