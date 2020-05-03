using Microsoft.Win32;
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
using WPF_Soundboard.Gui.Controls;

namespace WPF_Soundboard.Gui
{
    /// <summary>
    /// Interaction logic for ClipSettings.xaml
    /// </summary>
    public partial class ClipSettings : Window
    {
        private readonly int x;
        private readonly int y;
        private readonly ClipButton clipButton;
        private readonly ClipPageList clipPages;
        private readonly Clip clip;

        private bool initializing = true;
        private bool changed = false;
        private bool Changed
        {
            get => changed;
            set
            {
                if (!initializing)
                    changed = value;
                if (ApplyButton != null)
                    ApplyButton.IsEnabled = value;
            }
        }

        public ClipSettings(int x, int y, ClipButton clipButton, ClipPageList clipPages)
        {
            Clip clip = clipButton.Clip;

            DataContext = clip;
            InitializeComponent();
            this.x = x;
            this.y = y;
            this.clipButton = clipButton;
            this.clipPages = clipPages;
            this.clip = clip;

            HotkeyButton.Info = clip.HotkeyInfo;
            GlobalHotkeyButton.Info = clipPages.GlobalHotkeyList[x, y]?.GlobalHotkeyInfo.HotkeyInfo ?? new HotkeyInfo();
            ColorPicker.SelectedColor = clip.GuiInfo.Color;

            Name.Text = clip.GuiInfo.Name;
            PlayBehaviorSelector.SelectedIndex = (int)clip.SoundInfo.Behavior;
            CacheCheckbox.IsChecked = clip.SoundInfo.Cached;
            SinglePlayBox.IsChecked = clip.SoundInfo.SinglePlay;
            SinglePlayImmunityBox.IsChecked = clip.SoundInfo.SinglePlayImmunity;
            PlayLength.Text = clip.SoundInfo.Length.ToString();
            StartTime.Text = clip.SoundInfo.Start.ToString();
            Path.Text = clip.SoundInfo.Path;
            VolumeSlider.Value = clip.SoundInfo.Enabled ? clip.SoundInfo.VolumeModifier : 1;
            initializing = false;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Changed)
            {
                MessageBoxResult result = MessageBox.Show("You have unsaved changes. Do you want to save?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveChanges();
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void SaveChanges()
        {
            clip.GuiInfo = new GuiInfo()
            {
                Name = Name.Text,
                Color = ColorPicker.SelectedColor
            };
            clip.HotkeyInfo = HotkeyButton.Info;
            clip.SoundInfo = new SoundInfo
            {
                Behavior = (SoundInfo.PlayBehavior)PlayBehaviorSelector.SelectedIndex,
                Cached = CacheCheckbox.IsChecked ?? false,
                SinglePlay = SinglePlayBox.IsChecked ?? false,
                SinglePlayImmunity = SinglePlayImmunityBox.IsChecked ?? false,
                Enabled = !string.IsNullOrEmpty(Path.Text),
                Length = float.Parse(PlayLength.Text),
                Start = float.Parse(StartTime.Text),
                Path = Path.Text,
                VolumeModifier = (float)VolumeSlider.Value
            };
            if (GlobalHotkeyButton.Info.Enabled)
                clipPages.GlobalHotkeyList.AddHotkey(x, y, GlobalHotkeyButton.Info);
            else
                clipPages.GlobalHotkeyList.Remove((x, y));
            Changed = false;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3;*.wav;*.aiff)|*.mp3;*.wav;*.aiff|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Path.Text = openFileDialog.FileName;
                Name.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                Changed = true;
            }
        }


        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ApplyButton_Click(object sender, RoutedEventArgs e) => SaveChanges();


        private void ColorPicker_SelectedColorChanged(Color obj) => Changed = true;
        private void HotkeyButton_HotkeyChanged(object sender, EventArgs e) => Changed = true;
        private void Name_TextChanged(object sender, TextChangedEventArgs e) => Changed = true;
        private void TextChanged(object sender, TextChangedEventArgs e) => Changed = true;
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => Changed = true;
        private void PlayBehaviorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => Changed = true;
        private void CacheCheckbox_Checked(object sender, RoutedEventArgs e) => Changed = true;
    }
}
