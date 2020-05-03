using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for OtherSettings.xaml
    /// </summary>
    public partial class OtherSettings : Window
    {
        private readonly ClipPageList clipPages;
        bool initializing = true;

        public OtherSettings(ClipPageList clipPages)
        {
            InitializeComponent();
            this.clipPages = clipPages;
            HotkeyButton.Info = clipPages.StopHotkeyInfo;
            AutostartBox.IsChecked = clipPages.AutoStart;
            initializing = false;
        }

        private void HotkeyButton_HotkeyChanged(object sender, EventArgs e) => clipPages.StopHotkeyInfo = HotkeyButton.Info;

        private void AutostartBox_Checked(object sender, RoutedEventArgs e)
        {
            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            if (!key.GetValueNames().Contains("WPF_Soundboard"))
                key.SetValue("WPF_Soundboard", System.Reflection.Assembly.GetExecutingAssembly().Location);
            clipPages.AutoStart = true;
        }

        private void AutostartBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            if (!key.GetValueNames().Contains("WPF_Soundboard"))
                key.DeleteValue("WPF_Soundboard");
            clipPages.AutoStart = false;
        }
    }
}
