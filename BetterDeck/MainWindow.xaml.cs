using BetterDeck.Classes;
using BetterDeck.Classes.Actions.System;
using BetterDeck.Handler;
using BetterDeck.Manager;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BetterDeck
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainWindow window;
		public static System.Windows.Forms.NotifyIcon ni;

		public static Thread streamDeckThread;
		public static Thread teamspeakThread;

		public MainWindow()
		{
			// WPF
			InitializeComponent();

			// Set as window
			window = this;

			// Start stream deck connection thread
			streamDeckThread = new Thread(new ThreadStart(StreamDeckHandler.CheckStreamDeckConnection));
			streamDeckThread.Start();

			// Start teamspeak thread
			teamspeakThread = new Thread(new ThreadStart(TeamspeakHandler.CheckTeamspeakConnection));
			teamspeakThread.Start();

			// Minimize to tray
			ni = new System.Windows.Forms.NotifyIcon();
			ni.Icon = new Icon("Images/icon.ico");
			ni.Text = "BetterDeck";
			ni.Visible = true;
			ni.DoubleClick += OnNotifyIconDoubleClicked;

			// First row
			DeckButton.buttons.Add(new DeckButton(button1, 4));
			DeckButton.buttons.Add(new DeckButton(button2, 3));
			DeckButton.buttons.Add(new DeckButton(button3, 2));
			DeckButton.buttons.Add(new DeckButton(button4, 1));
			DeckButton.buttons.Add(new DeckButton(button5, 0));

			// Second row
			DeckButton.buttons.Add(new DeckButton(button6, 9));
			DeckButton.buttons.Add(new DeckButton(button7, 8));
			DeckButton.buttons.Add(new DeckButton(button8, 7));
			DeckButton.buttons.Add(new DeckButton(button9, 6));
			DeckButton.buttons.Add(new DeckButton(button10, 5));

			// Third row
			DeckButton.buttons.Add(new DeckButton(button11, 14));
			DeckButton.buttons.Add(new DeckButton(button12, 13));
			DeckButton.buttons.Add(new DeckButton(button13, 12));
			DeckButton.buttons.Add(new DeckButton(button14, 11));
			DeckButton.buttons.Add(new DeckButton(button15, 10));

			DeckButton.SetAllToEmpty();

			// Search for profiles
			ProfileHandler.SearchForProfiles();

			// Init action names list
			ActionManager.InitActionNamesList();

			// Register all actions
			ActionManager.RegisterAction(new TypeText());
			ActionManager.RegisterAction(new PressKey());
			ActionManager.RegisterAction(new MuteOutput());
			ActionManager.RegisterAction(new MuteInput());

			// Init all key codes
			KeyManager.InitKeyCodes();
		}

		protected override void OnStateChanged(EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
				Hide();

			base.OnStateChanged(e);
		}

		private void OnNotifyIconDoubleClicked(object sender, EventArgs args)
		{
			Show();
			WindowState = WindowState.Normal;
		}

		private void Image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// Get clicked deck button and set as selected
			DeckButton deckButton = DeckButton.GetByWPF((System.Windows.Controls.Image)sender);
			deckButton.SetSelected(true);

			// Show image tab
			imageTab.Visibility = Visibility.Visible;

			// Show actions tab
			actionsTab.Visibility = Visibility.Visible;

			// Select image tab
			imageTab.IsSelected = true;
		}

		private void OnSaveProfileClicked(object sender, RoutedEventArgs e)
		{
			int selectedIndex = ProfileHandler.GetSelectedProfileIndex();

			if (selectedIndex == 0) // "Create a new profile..." selected
			{
				ProfileHandler.CreateNewProfile();
			}
			else // Save profile
			{
				ProfileHandler.SaveProfile();
			}
		}

		private void OnLoadProfileClicked(object sender, RoutedEventArgs e)
		{
			if (profiles.SelectedIndex == 0) // Trying to load "Create a new profile..." => Load empty profile
			{
				MessageBoxResult result = MessageBox.Show("Do you want to reset everything back to default?\n" +
					"All unsaved changes will get lost!", "Load profile", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

				if (result != MessageBoxResult.Yes)
					return;

				// Load an empty profile
				ProfileHandler.LoadDefaults();
			}
			else // Loading a saved profile
			{
				// Get selected profile index
				int profileIndex = ProfileHandler.GetSelectedProfileIndex();
				ProfileHandler.LoadProfile(profileIndex);
			}
		}

		private void OnDeleteProfileClicked(object sender, RoutedEventArgs e)
		{
			int selectedIndex = ProfileHandler.GetSelectedProfileIndex();

			// Ignore if trying to load "Create a new profile..."
			if (selectedIndex == 0)
			{
				MessageBox.Show("You can't delete an empty profile!", "Delete profile", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			ProfileHandler.DeleteProfile(selectedIndex);
		}

		private void ChangeImage_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();

			openFile.Title = "Select image";
			openFile.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			openFile.RestoreDirectory = true;

			if (openFile.ShowDialog() == true)
			{
				string filePath = openFile.FileName;
				DeckButton.GetSelectedButton().SetImage(filePath);
			}
		}

		private void ClearImage_Click(object sender, RoutedEventArgs e)
		{
			DeckButton.GetSelectedButton().SetImage("");
		}

		private void ImageText_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Get currently selected key
			DeckButton.GetSelectedButton().SetText(imageText.Text);
		}

		private void ImageTextFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Get currently selected key
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null || imageTextFontSize.Value == null)
				return;

			// Set image size
			selectedButton.SetTextFontSize((int)imageTextFontSize.Value);
		}

		private void ImageTextOffsetLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Get currently selected key
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null || imageTextOffsetLeft.Value == null)
				return;

			// Set offset left
			selectedButton.SetTextOffsetLeft((int)imageTextOffsetLeft.Value);
		}

		private void ImageTextOffsetTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Get currently selected key
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null || imageTextOffsetTop.Value == null)
				return;

			// Set offset top
			selectedButton.SetTextOffsetTop((int)imageTextOffsetTop.Value);
		}

		private void OnSelectedImageFontColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
		{
			// Get currently selected key
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null)
				return;

			// Set background color
			selectedButton.SetFontColor((System.Windows.Media.Color)imageFontColor.SelectedColor);
		}

		private void OnSelectedImageBackgroundColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
		{
			// Get currently selected key
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null)
				return;

			// Set background color
			selectedButton.SetBackgroundColor((System.Windows.Media.Color)imageBackgroundColor.SelectedColor);
		}

		private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (brightnessLabel == null)
				return;

			// Update brightness without updating the slider (would be unnecessary as the user is adjusting the brightness with the slider already)
			BrightnessHandler.SetBrightness((int)brightnessSlider.Value, false);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Interrupt threads
			streamDeckThread.Interrupt();
			teamspeakThread.Interrupt();

			// Stream deck
			if (StreamDeckHandler.IsStreamDeckConnected())
			{
				StreamDeckHandler.GetStreamDeck().SetBrightness(0);
				StreamDeckHandler.Dispose();
			}

			// Window notify icon
			ni.Visible = false;
			ni.Dispose();
		}

		private void AddAction_Click(object sender, RoutedEventArgs e)
		{
			DeckButton.GetSelectedButton().AddAction();
		}

		private void Debug_TriggerActionsClick(object sender, RoutedEventArgs e)
		{
			if (DeckButton.GetSelectedButton() == null)
					return;

			DeckButton.GetSelectedButton().ExecuteActions(true);
		}
	}
}
