using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
using WPF_Soundboard.Audio;
using WPF_Soundboard.Clips;
using WPF_Soundboard.Gui;
using WPF_Soundboard.Gui.Controls;

namespace WPF_Soundboard
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ClipPageList clipPages;
		ClipPage currentPage;
		NotifyIcon notifyIcon = new NotifyIcon();
		bool closing = false;

		public MainWindow()
		{
#if !DEBUG
            Hide();
#endif

			Assembly assembly = Assembly.GetExecutingAssembly();
			string resourceName = "WPF_Soundboard.Resources.icon.ico";

			using Stream stream = assembly.GetManifestResourceStream(resourceName);
			notifyIcon.Icon = new Icon(stream);
			notifyIcon.Visible = true;
			notifyIcon.MouseClick += NotifyIcon_MouseClick;

			notifyIcon.ContextMenuStrip = new ContextMenuStrip();
			ToolStripMenuItem showItem = new ToolStripMenuItem("Show");
			showItem.Click += ShowItem_Click;
			ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
			exitItem.Click += ExitItem_Click;
			notifyIcon.ContextMenuStrip.Items.Add(showItem);
			notifyIcon.ContextMenuStrip.Items.Add(exitItem);


			InitializeComponent();
			try
			{
				AudioHandler.Initialize();
			}
			catch
			{
				//TODO Error Handling
				throw;
			}

			clipPages = Serializer.GetClipPages();
			clipPages.GlobalHotkeyList.OnHotkeyPressed += GlobalHotkeyList_OnHotkeyPressed;
			clipPages.OnStopHotkeyPressed += ClipPages_OnStopHotkeyPressed;

			if (clipPages.Count == 0)
				clipPages.Add(new ClipPage(7, 5));

			foreach (ClipPage page in clipPages)
			{
				page.OnChanged += Page_OnChanged;
				page.OnHotkeyPressed += Page_OnHotkeyPressed;
			}

			currentPage = clipPages[0];
			BuildClipPage();
			UpdatePagesList();
		}

		private void ClipPages_OnStopHotkeyPressed(object sender, EventArgs e) => AudioHandler.StopAll();
		private void GlobalHotkeyList_OnHotkeyPressed(object sender, GlobalHotkeyEventArgs e) => currentPage[e.X, e.Y].Play();

		private void Page_OnHotkeyPressed(object sender, EventArgs e)
		{
			currentPage = sender as ClipPage;
			BuildClipPage();
		}

		private void ExitItem_Click(object sender, EventArgs e)
		{
			closing = true;
			notifyIcon.Visible = false;
			AudioHandler.Dispose();
			System.Windows.Application.Current.Shutdown();
		}

		private void ShowItem_Click(object sender, EventArgs e)
		{
			Show();
			Activate();
		}

		private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Show();
				Activate();
			}
		}

		private void UpdatePagesList()
		{
			PagesList.Items.Clear();
			foreach (ClipPage page in clipPages)
			{
				PagesList.Items.Add(page);
			}
		}

		private void BuildClipPage()
		{
			using (System.Windows.Threading.DispatcherProcessingDisabled d = Dispatcher.DisableProcessing())
			{
				SoundButtonContainer.Children.Clear();
				SoundButtonContainer.RowDefinitions.Clear();
				SoundButtonContainer.ColumnDefinitions.Clear();
				for (int x = 0; x < currentPage.Width; x++)
				{
					SoundButtonContainer.ColumnDefinitions.Add(new ColumnDefinition());
				}
				for (int y = 0; y < currentPage.Height; y++)
				{
					SoundButtonContainer.RowDefinitions.Add(new RowDefinition());
				}

				for (int x = 0; x < currentPage.Width; x++)
				{

					for (int y = 0; y < currentPage.Height; y++)
					{

						ClipButton button = new ClipButton(x, y, currentPage[x, y], clipPages);
						if (currentPage[x, y] == null)
							currentPage[x, y] = button.Clip;
						button.OnDragDrop += Button_OnDragDrop;
						Grid.SetColumn(button, x);
						Grid.SetRow(button, y);
						SoundButtonContainer.Children.Add(button);

					}
				}
			}
		}



		private void Button_OnDragDrop(object sender, ClipButtonDragEventArgs e)
		{
			Clip temp = currentPage[e.DestinationCoordinates.x, e.DestinationCoordinates.y];
			currentPage[e.DestinationCoordinates.x, e.DestinationCoordinates.y] = currentPage[e.SourceCoordinates.x, e.SourceCoordinates.y];
			currentPage[e.SourceCoordinates.x, e.SourceCoordinates.y] = temp;
			e.Source.Clip = temp;
			e.Destination.Clip = currentPage[e.DestinationCoordinates.x, e.DestinationCoordinates.y];
		}

		private void AddClipPage_Click(object sender, RoutedEventArgs e)
		{
			ClipPage page = new ClipPage(7, 5) { Name = "Page " + clipPages.Count };
			page.OnChanged += Page_OnChanged;

			if (PagesList.SelectedIndex == -1)
			{
				clipPages.Add(page);
				UpdatePagesList();
				PagesList.SelectedIndex = clipPages.Count - 1;
			}
			else
			{
				clipPages.Insert(PagesList.SelectedIndex, page);
				UpdatePagesList();
			}
		}

		private void RemoveClipPage_Click(object sender, RoutedEventArgs e)
		{
			if (PagesList.SelectedIndex == -1)
				return;
			clipPages.RemoveAt(PagesList.SelectedIndex);
			UpdatePagesList();
		}

		private void EditClipPage_Click(object sender, RoutedEventArgs e) => new ClipPageSettings(currentPage) { Left = Left + 50, Top = Top + 50 }.Show();


		private void StopButton_Click(object sender, RoutedEventArgs e) => AudioHandler.StopAll();

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Save();
			if (closing)
				return;
			e.Cancel = true;
			Hide();
		}

		private void Page_OnChanged(object sender, EventArgs e)
		{
			if (sender == currentPage)
				BuildClipPage();
			UpdatePagesList();
		}

		private void PagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PagesList.SelectedIndex != -1)
				currentPage = clipPages[PagesList.SelectedIndex];
			BuildClipPage();
			PagesList.SelectedIndex = -1;
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => AudioHandler.ClipVolume = (float)VolumeSlider.Value;

		private void AudioConfigButton_Click(object sender, RoutedEventArgs e) => new AudioSettings() { Left = Left + 50, Top = Top + 50 }.Show();

		private void SettingsButton_Click(object sender, RoutedEventArgs e) => new OtherSettings(clipPages) { Left = Left + 50, Top = Top + 50 }.Show();

		~MainWindow()
		{
			Properties.Settings.Default.Save();
		}
	}
}

