using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Gui.Controls
{
    /// <summary>
    /// Interaction logic for ClipButton.xaml
    /// </summary>
    public partial class ClipButton : UserControl
    {
        private readonly int x;
        private readonly int y;
        private readonly ClipPageList clipPages;
        private Clip clip;
        private Point startPoint;

        public event EventHandler<ClipButtonDragEventArgs> OnDragDrop;

        public new Clip Clip
        {
            get => clip;
            set
            {
                clip = value;
                Update();
            }
        }

        public ClipButton(int x, int y, Clip clip, ClipPageList clipPages)
        {
            InitializeComponent();
            this.x = x;
            this.y = y;
            this.clipPages = clipPages;
            this.clip = clip ?? new Clip()
            {
                GuiInfo = new GuiInfo()
                {
                    Name = "<empty>",
                    Color = Color.FromRgb(221, 221, 221)
                },
                SoundInfo = new SoundInfo()
                {
                    SinglePlay = true,
                }
            };
            DataContext = Clip;
            Update();
            Clip.OnChanged += Clip_OnChanged;
        }

        private void Clip_OnChanged(object sender, EventArgs e) => Update();

        private void Update()
        {
            Button.Background = new SolidColorBrush(Clip.GuiInfo.Color);

            byte colorAverage = (byte)((Clip.GuiInfo.Color.R + Clip.GuiInfo.Color.G + Clip.GuiInfo.Color.B) / 3);

            if (colorAverage >= 128) // color is light, choose a dark foreground
                Button.Foreground = new SolidColorBrush(Colors.Black);
            else // color is dark, choose a light foreground
                Button.Foreground = new SolidColorBrush(Colors.White);

            string text = Clip.GuiInfo.Name;
            if (Clip.HotkeyInfo.Enabled)
                text += "\nl: " + Clip.HotkeyInfo.ToString();
            if (clipPages.GlobalHotkeyList[x, y]?.IsEnabled ?? false)
                text += "\n" + clipPages.GlobalHotkeyList[x, y].GlobalHotkeyInfo.HotkeyInfo.ToString();

            TextBlock.Text = text;

        }


        private void Button_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Effects == System.Windows.DragDropEffects.None)
                return;

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                Clip.SoundInfo = new SoundInfo()
                {
                    Behavior = Clip.SoundInfo.Behavior,
                    Cached = Clip.SoundInfo.Cached,
                    SinglePlay = true,
                    VolumeModifier = 1,
                    Enabled = true,
                    Path = files[0]
                };
                Clip.GuiInfo = new GuiInfo()
                {
                    Color = Clip.GuiInfo.Color,
                    Name = System.IO.Path.GetFileNameWithoutExtension(files[0])
                };
            }
            else if (e.Data.GetDataPresent("clipButtonCoordinates"))
            {
                (int x, int y) source = ((int x, int y))e.Data.GetData("clipButtonCoordinates");
                OnDragDrop.Invoke(this, new ClipButtonDragEventArgs()
                {
                    Source = (ClipButton)e.Data.GetData("clipButtonSource"),
                    SourceCoordinates = source,
                    Destination = this,
                    DestinationCoordinates = (x, y)
                });
            }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => startPoint = e.GetPosition(null);

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DataObject dragData = new DataObject("clipButtonCoordinates", (x, y));
                dragData.SetData("clipButtonSource", this);
                DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move);
            }
        }

        private void Button_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop) || e.Data.GetDataPresent("clipButtonSource")) ||
                (e.Data.GetDataPresent("clipButtonSource") && e.Data.GetData("clipButtonSource") == this))
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Clip.Play();

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Clip.GuiInfo = new GuiInfo()
            {
                Name = Clip.GuiInfo.Name,
                Color = (Color)ColorConverter.ConvertFromString((string)(sender as MenuItem).Header)
            };
            Update();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e) => new ClipSettings(x, y, this, clipPages)
        {
            Left = Application.Current.MainWindow.Left + 50,
            Top = Application.Current.MainWindow.Top + 50,
        }.Show();

    }
}
