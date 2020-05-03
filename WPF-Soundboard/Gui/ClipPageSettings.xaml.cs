using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Gui
{
    /// <summary>
    /// Interaction logic for ClipPageSettings.xaml
    /// </summary>
    public partial class ClipPageSettings : Window
    {
        private readonly ClipPage clipPage;

        public ClipPageSettings(ClipPage clipPage)
        {
            InitializeComponent();
            Name.Text = clipPage.Name;
            Width.Text = clipPage.Width.ToString();
            Height.Text = clipPage.Height.ToString();
            Hotkey.Info = clipPage.HotkeyInfo;
            this.clipPage = clipPage;
        }



        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Size_LostFocus(object sender, RoutedEventArgs e) => clipPage.ChangeSize(int.Parse(Width.Text), int.Parse(Height.Text));

        private void Size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                clipPage.ChangeSize(int.Parse(Width.Text), int.Parse(Height.Text));
            }
        }

        private void Hotkey_HotkeyChanged(object sender, EventArgs e) => clipPage.HotkeyInfo = Hotkey.Info;

        private void Name_LostFocus(object sender, RoutedEventArgs e) => clipPage.Name = Name.Text;

        private void Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                clipPage.Name = Name.Text;
        }
    }
}
