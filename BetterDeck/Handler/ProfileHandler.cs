using BetterDeck.Classes.Actions;
using BetterDeck.Handler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BetterDeck.Classes
{
	public class ProfileHandler
	{
		private static string loadedProfileName = "";
		private const string PATH = "./Profiles/";
		private const string EXTENSION = ".txt";

		public static void SearchForProfiles()
		{
			// Remove all items from profiles list (except first one)
			while (MainWindow.window.profiles.Items.Count > 1)
			{
				// Remove item
				MainWindow.window.profiles.Items.RemoveAt(1);
			}

			// Search for profiles
			string[] files = Directory.GetFiles(PATH);
		
			foreach (string file in files)
			{
				if (!file.EndsWith(EXTENSION))
					continue;

				// Create new combo box item and add it to the list
				ComboBoxItem item = new ComboBoxItem();
				int extensionLength = EXTENSION.Length;
				string profileName = file.Remove(file.Length - extensionLength, extensionLength).Remove(0, PATH.Length);
				item.Content = profileName;
				MainWindow.window.profiles.Items.Add(item);
			}
		}

		public static string GetLoadedProfileName()
		{
			return loadedProfileName;
		}

		public static void CreateNewProfile()
		{
			// Show a text box window
			TextBoxWindow profileWindow = new TextBoxWindow();
			profileWindow.Owner = Window.GetWindow(MainWindow.window);
			profileWindow.ShowDialog();

			// Get the entered profile name
			string profileName = profileWindow.text;

			// Save to profile name (if OK was clicked)
			if (profileName.Length > 0)
			{
				if (GetProfileIndexByName(profileName) != -1) // Current profile name is already given
				{
					MessageBoxResult result = MessageBox.Show($"A profile named '{profileName}' already exists!\n" +
						"Do you want to overwrite it with this profile?",
						"Save profile", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

					// Don't save profile
					if (result != MessageBoxResult.Yes)
						return;
				}

				// Save profile with the entered name
				SaveProfile(profileName);
			}
		}

		public static void LoadProfile(int profileIndex)
		{
			// Get profile name
			string profileName = ((ComboBoxItem)MainWindow.window.profiles.Items.GetItemAt(profileIndex)).Content.ToString();

			try
			{
				string profileData = "";

				// Read file
				using (StreamReader reader = new StreamReader(PATH + profileName + EXTENSION))
				{
					profileData = reader.ReadToEnd();
				}

				// Deserialize profile data
				var jset = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects};
				Profile profile = JsonConvert.DeserializeObject<Profile>(profileData, jset);

				// Set brightness
				BrightnessHandler.SetBrightness(profile.brightness);

				// Get currently selected button
				DeckButton selectedButton = DeckButton.GetSelectedButton();

				// Load buttons
				int i = 0;
				foreach (DeckButton button in DeckButton.buttons)
				{
					// Remove all actions from button
					button.RemoveAllActions();

					// Select the button (to avoid errors while changing actions)
					if (DeckButton.GetSelectedButton() != null)
					{
						DeckButton.GetSelectedButton().SetSelected(false);
					}

					button.SetSelected(true);

					// Get the saved button
					SaveDeckButton saveDeckButton = profile.buttons.ElementAt(i++);

					// Set actions
					foreach (Action action in saveDeckButton.actions)
					{
						button.AddAction(action);
					}

					// Set image
					button.SetImage(saveDeckButton.imagePath, false);

					// Set text
					button.SetText(saveDeckButton.imageText, false);

					// Set text font size
					button.SetTextFontSize(saveDeckButton.textFontSize, false);

					// Set text offset left
					button.SetTextOffsetLeft(saveDeckButton.textOffsetLeft, false);

					// Set text offset top
					button.SetTextOffsetTop(saveDeckButton.textOffsetTop, false);

					// Set font color
					button.SetFontColor(saveDeckButton.fontColor, true);

					// Set background color
					button.SetBackgroundColor(saveDeckButton.backgroundColor, true);
				}

				// Select currently selected button again to refresh the tab contents
				if (selectedButton != null)
				{
					selectedButton.SetSelected(true);
				}
				else
				{
					DeckButton.GetSelectedButton().SetSelected(false);
				}

				// Send message
				MessageBox.Show($"Your profile '{profileName}' was successfully loaded!", "Load profile", MessageBoxButton.OK, MessageBoxImage.Information);

				// Save currently loaded profile name
				loadedProfileName = profileName;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"There was an error loading the profile '{profileName}'!", "Load profile", MessageBoxButton.OK, MessageBoxImage.Error);
				MessageBox.Show(ex.ToString());
			}
		}

		public static void SaveProfile(string profileName = "")
		{
			// Selected profile index
			int profileIndex = GetSelectedProfileIndex();

			// Create & Serialize profile
			Profile saveProfile = new Profile().CreateNewProfile();
			var jset = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects};
			string profileString = JsonConvert.SerializeObject(saveProfile, jset);

			// Save to file
			if (profileName.Length == 0) // Saving an already existing profile
			{
				profileName = ((ComboBoxItem)MainWindow.window.profiles.Items.GetItemAt(profileIndex)).Content.ToString();

				if (loadedProfileName != profileName)
				{
					MessageBoxResult result = MessageBox.Show($"You are about to overwrite the profile '{profileName}' with your current profile!\n" +
							"Do you want to continue?",
							"Save profile", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

					// Don't save profile
					if (result != MessageBoxResult.Yes)
						return;
				}
			}

			string path = PATH + profileName + EXTENSION;

			try
			{
				using (var writer = new StreamWriter(path, false))
				{
					writer.WriteLine(profileString);
				}

				// Scan for all profiles
				SearchForProfiles();

				// Select saved profile
				profileIndex = GetProfileIndexByName(profileName);
				SelectProfile(profileIndex);

				// Save current profile name
				loadedProfileName = profileName;

				// Show info
				MessageBox.Show($"Your profile '{profileName}' was successfully saved!", "Save profile", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show("Your profile name contains illegal characters!", "Save profile", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public static void DeleteProfile(int profileIndex)
		{
			string profileName = GetProfileNameByIndex(profileIndex);

			MessageBoxResult result = MessageBox.Show($"Are you sure that you want to delete the profile '{profileName}'?\n" +
				"This process can't be undone!", "Delete profile", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

			// Abort if user doesn't want to delete the profile
			if (result != MessageBoxResult.Yes)
				return;

			// Remove profile from files
			string path = PATH + profileName + EXTENSION;

			try
			{
				File.Delete(path);

				// Remove profile from profiles list
				MainWindow.window.profiles.Items.RemoveAt(profileIndex);

				// Select "Create new profile..." again
				MainWindow.window.profiles.SelectedIndex = 0;

				// Reset the loaded profile if the loaded profile was deleted
				if (loadedProfileName == profileName)
				{
					loadedProfileName = "";
				}

				// Show info
				MessageBox.Show($"Profile '{profileName}' was successfully deleted!", "Save profile", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				MessageBox.Show($"There was an error deleting the profile '{profileName}'!", "Delete profile", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public static void LoadDefaults()
		{
			// Set brightness to default
			BrightnessHandler.SetBrightness(BrightnessHandler.DEFAULT_BRIGHTNESS);

			// Reset all buttons back to default
			foreach (DeckButton button in DeckButton.buttons)
			{
				button.ResetToDefault();
			}
		}

		public static void SelectProfile(int profileIndex)
		{
			MainWindow.window.profiles.SelectedIndex = profileIndex;
		}

		public static int GetSelectedProfileIndex()
		{
			return MainWindow.window.profiles.SelectedIndex;
		}

		public static string GetSelectedProfileName()
		{
			return ((ComboBoxItem)MainWindow.window.profiles.SelectedItem).Content.ToString();
		}

		public static int GetProfileIndexByName(string profileName)
		{
			// Find profile index
			int index;

			for (index = 0; index < MainWindow.window.profiles.Items.Count; index++)
			{
				if (((ComboBoxItem)MainWindow.window.profiles.Items.GetItemAt(index)).Content.ToString() == profileName)
					break;
			}

			// Return "-1" if no profile was found
			if (index >= MainWindow.window.profiles.Items.Count)
				index = -1;

			return index;
		}

		public static string GetProfileNameByIndex(int profileIndex)
		{
			return ((ComboBoxItem)(MainWindow.window.profiles.Items.GetItemAt(profileIndex))).Content.ToString();
		}
	}

	public class Profile
	{
		public int brightness { get; set; }
		public List<SaveDeckButton> buttons { get; set; }

		public Profile()
		{
			buttons = new List<SaveDeckButton>();
		}

		public Profile CreateNewProfile()
		{
			// Get set brightness
			brightness = BrightnessHandler.GetBrightness();

			// Get important button values
			foreach (DeckButton button in DeckButton.buttons)
			{
				List<Action> actions = new List<Action>();

				// Get all actions out of the action elements (as the elements contain unimportant information)
				foreach (ActionElement actionElement in button.actionElements)
				{
					actions.Add(actionElement.action);
				}

				SaveDeckButton saveDeckButton = new SaveDeckButton(button.streamDeckKeyID, button.imagePath, button.imageText, button.textFontSize, button.textOffsetLeft, button.textOffsetTop, button.fontColor, button.backgroundColor, actions);
				buttons.Add(saveDeckButton);
			}

			return this;
		}
	}
}
