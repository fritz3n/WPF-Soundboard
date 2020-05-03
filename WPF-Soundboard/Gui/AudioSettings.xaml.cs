using System;
using System.Collections.Generic;
using System.Linq;
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
using WPF_Soundboard.Audio;
using WPF_Soundboard.Gui.Audio;

namespace WPF_Soundboard.Gui
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class AudioSettings : Window
    {
        private bool changed = false;
        private bool initializing = true;

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

        public AudioSettings()
        {
            InitializeComponent();
            foreach (KeyValuePair<string, string> pair in WasapiGui.GetInputs())
            {
                InputListBox.Items.Add(new CheckBox()
                {
                    Content = pair.Value,
                    Tag = pair.Key,
                    IsChecked = AudioHandler.Config.WasapiIns.Contains(pair.Key)
                });
            }
            VolumeSlider.Value = AudioHandler.Config.Volume;
            OutputTypeBox.SelectedIndex = (int)AudioHandler.Config.OutType;
            initializing = false;
        }

        private void VolumenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => Changed = true;

        private void RefreshOutputSettings()
        {
            switch (OutputTypeBox.SelectedIndex)
            {
                case -1:
                    WasapiSection.Visibility = Visibility.Hidden;
                    AsioSection.Visibility = Visibility.Hidden;
                    break;
                case 0:
                    WasapiOut.Items.Clear();
                    foreach (KeyValuePair<string, string> pair in WasapiGui.GetOutputs())
                    {

                        WasapiOut.Items.Add(pair);

                        if (AudioHandler.Config.OutputParameters.ContainsKey("OutputName") && (string)AudioHandler.Config.OutputParameters["OutputName"] == pair.Key)
                            WasapiOut.SelectedItem = pair;
                    }
                    WasapiSection.Visibility = Visibility.Visible;
                    AsioSection.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    AsioDriver.Items.Clear();
                    foreach (string driver in AsioGui.GetDrivers())
                    {
                        AsioDriver.Items.Add(driver);
                        if (AudioHandler.Config.OutputParameters.ContainsKey("DriverName") && (string)AudioHandler.Config.OutputParameters["DriverName"] == driver)
                            AsioDriver.SelectedItem = driver;
                    }
                    AsioStartChannel.Text = AudioHandler.Config.OutputParameters.ContainsKey("StartChannel") ?
                                            (string)AudioHandler.Config.OutputParameters["StartChannel"] : "0";

                    AsioChannelCount.Text = AudioHandler.Config.OutputParameters.ContainsKey("Channels") ?
                                            (string)AudioHandler.Config.OutputParameters["Channels"] : "2";
                    WasapiSection.Visibility = Visibility.Hidden;
                    AsioSection.Visibility = Visibility.Visible;
                    break;

            }
        }

        private void SaveChanges()
        {
            AudioConfig config = new AudioConfig
            {
                Volume = (float)VolumeSlider.Value,
                WasapiIns = InputListBox.Items.Cast<CheckBox>().Where(box => box.IsChecked ?? false).Select(box => box.Tag as string).ToArray(),
                OutType = (AudioConfig.OutputType)OutputTypeBox.SelectedIndex
            };
            switch (OutputTypeBox.SelectedIndex)
            {
                case 0:
                    if (WasapiOut.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select a Wasapi output.");
                        return;
                    }
                    config.OutputParameters = new Dictionary<string, object>()
                    {
                        {"OutputName", ((KeyValuePair<string, string>)WasapiOut.SelectedItem).Key}
                    };
                    break;
                case 1:
                    if (AsioDriver.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please select a Wasapi output.");
                        return;
                    }
                    config.OutputParameters = new Dictionary<string, object>()
                    {
                        {"DriverName", AsioDriver.SelectedItem as string ?? "<unknown>"},
                        {"StartChannel", int.Parse(AsioStartChannel.Text) },
                        {"Channels", int.Parse(AsioChannelCount.Text) }
                    };
                    break;
            }
            AudioHandler.Config = config;
            Changed = false;
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
        private void AsioDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AsioDriver.SelectedIndex == -1)
            {
                AsioInfoLabel.Content = "";
            }
            else
            {
                AsioInfoLabel.Content = "No. of channels: " + AsioGui.GetChannels(AsioDriver.SelectedItem as string).Length;
            }
        }
        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OutputTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshOutputSettings();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ApplyButton_Click(object sender, RoutedEventArgs e) => SaveChanges();

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) => Changed = true;

        private void TextChanged(object sender, TextChangedEventArgs e) => Changed = true;


    }
}
