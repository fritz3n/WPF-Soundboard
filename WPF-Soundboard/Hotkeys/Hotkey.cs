using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace WPF_Soundboard.Hotkeys
{
    public class Hotkey : IDisposable
    {
        static KeyboardHook hook = new KeyboardHook();

        public static bool Enabled { get; set; } = true;

        public ModifierKeys Modifier { get; private set; }
        public Keys Key { get; private set; }

        [JsonIgnore]
        public bool Registered { get; private set; } = false;
        [JsonIgnore]
        int Id { get; set; } = -1;

        public event EventHandler<EventArgs> Pressed;

        public Hotkey(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
            hook.KeyPressed += Hook_KeyPressed;
        }

        private void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (Enabled && e.Modifier == Modifier && e.Key == Key)
                Pressed?.Invoke(this, EventArgs.Empty);
        }

        public bool Register()
        {
            if (Registered)
                Unregister();

            try
            {
                Id = hook.RegisterHotKey(Modifier, Key);
                Registered = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Unregister()
        {
            if (!Registered)
                return;

            hook.UnregisterHotkey(Id);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => Unregister();

        ~Hotkey()
        {
            Dispose(false);
        }
    }
}
