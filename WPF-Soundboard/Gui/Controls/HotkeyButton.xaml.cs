using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Gui.Controls
{
    /// <summary>
    /// Interaction logic for HotkeyButton.xaml
    /// </summary>
    public partial class HotkeyButton : System.Windows.Controls.UserControl
    {
        bool recording = false;
        private HotkeyInfo info;

        public HotkeyInfo Info { get => info; set { info = value; UpdateText(); } }

        public event EventHandler HotkeyChanged;

        public HotkeyButton()
        {
            InitializeComponent();
            UpdateText();
        }

        private void UpdateText()
        {
            string text;
            if (Info.Enabled)
            {
                text = Info.ModifierKeys.ToString().Replace(",", " +");
                text += " + " + Info.Key.ToString();
            }
            else
            {
                text = "No Hotkey set.";
            }
            KeyButton.Content = text;
        }

        private void HotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            recording = !recording;

            if (recording)
            {
                KeyButton.Content = "Press keys to assign Hotkey";
            }
            else
            {
                UpdateText();
            }
        }

        private void CallHotkeyChanged() => HotkeyChanged?.Invoke(this, EventArgs.Empty);

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (!recording)
                return;

            // Filter out Modifier-Keys
            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.System ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin)
            {
                return;
            }

            if (e.Key == Key.Escape)
            {
                Info = new HotkeyInfo()
                {
                    Enabled = false
                };
                e.Handled = true;
                CallHotkeyChanged();
                UpdateText();
                recording = false;
                return;
            }


            e.Handled = true;
            Info = new HotkeyInfo()
            {
                Enabled = true,
                ModifierKeys = (WPF_Soundboard.Hotkeys.ModifierKeys)Keyboard.Modifiers,
                Key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key),
            };
            CallHotkeyChanged();
            UpdateText();
            recording = false;
        }
    }
}
