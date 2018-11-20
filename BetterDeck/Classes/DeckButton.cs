using BetterDeck.Classes.Actions;
using BetterDeck.Classes.Actions.System;
using BetterDeck.Handler;
using BetterDeck.Helper;
using BetterDeck.Manager;
using StreamDeckSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BetterDeck.Classes
{
	public class DeckButton
	{
		public static int ICON_SIZE = 72;
		public static int DEFAULT_TEXT_FONT_SIZE = 12;
		public static int DEFAULT_TEXT_OFFSET_LEFT = 36;
		public static int DEFAULT_TEXT_OFFSET_TOP = 36;
		public static Brush DEFAULT_FONT_COLOR = Brushes.White;
		public static Brush DEFAULT_BACKGROUND_COLOR = Brushes.Transparent;

		public static List<DeckButton> buttons = new List<DeckButton>();
		private static DeckButton selectedButton { get; set; }
		public System.Windows.Controls.Image image { get; set; }
		public int streamDeckKeyID { get; set; }
		public string imagePath { get; set; }
		public string imageText { get; set; }
		public int textFontSize = DEFAULT_TEXT_FONT_SIZE;
		public int textOffsetLeft = DEFAULT_TEXT_OFFSET_LEFT;
		public int textOffsetTop = DEFAULT_TEXT_OFFSET_TOP;
		public Brush fontColor = DEFAULT_FONT_COLOR;
		public Brush backgroundColor = DEFAULT_BACKGROUND_COLOR;
		public List<ActionElement> actionElements = new List<ActionElement>();

		public DeckButton(System.Windows.Controls.Image image, int streamDeckKeyID)
		{
			this.image = image;
			this.streamDeckKeyID = streamDeckKeyID;
		}

		public void SetImage(string imagePath, bool updateImage = true)
		{
			// Set image into button
			this.imagePath = imagePath;

			if (updateImage)
			{
				// Create bitmap
				Bitmap bitmap = CreateBitmap(imagePath);

				if (bitmap == null)
					return;

				// Set image in program
				image.Source = ImageSourceFromBitmap(bitmap);

				// Draw image to stream deck key
				UpdateStreamDeckImage(bitmap);

				// Update "image" tab
				ImageTabHandler.Update();
			}
		}

		private Bitmap CreateBitmap(string imagePath)
		{
			// Create graphics
			Bitmap bitmap = new Bitmap(ICON_SIZE, ICON_SIZE);
			Graphics g = Graphics.FromImage(bitmap);

			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			// Fill background with selected color
			g.FillRectangle(Brushes.Black, 0, 0, ICON_SIZE, ICON_SIZE);
			g.FillRectangle(backgroundColor, 0, 0, ICON_SIZE, ICON_SIZE);

			// Load image and draw it (if any)
			if (imagePath.Length > 0)
			{
				try
				{
					Image image = Image.FromFile(imagePath);
					g.DrawImage(image, 0, 0, ICON_SIZE, ICON_SIZE);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"The image '{imagePath}' could not be loaded: File not found!", "Load image", MessageBoxButton.OK, MessageBoxImage.Error);
					return null;
				}
			}

			// Draw text to image
			Font font = new Font("Arial", textFontSize, System.Drawing.FontStyle.Bold);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;

			g.DrawString(imageText, font, fontColor, new PointF(textOffsetLeft, textOffsetTop), stringFormat);

			return bitmap;
		}

		public void SetSelected(bool selected)
		{
			if (selected)
			{
				// Remove selection from current selected button
				if (selectedButton != null)
				{
					selectedButton.SetSelected(false);
				}

				// Set button as selected
				selectedButton = this;
			}

			// Add/Remove image border
			System.Windows.Media.Brush brush = ((selected) ? (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#1f80f3")
				: System.Windows.Media.Brushes.Transparent);

			((System.Windows.Controls.Border)image.Parent).BorderBrush = brush;

			if (selected) // => Selected
			{
				// Update "image" tab
				ImageTabHandler.Update();

				// Add all action elements of action to list
				foreach (ActionElement actionElement in actionElements)
				{
					// Add to list of actions (as second last item)
					MainWindow.window.actionsList.Children.Insert((MainWindow.window.actionsList.Children.Count - 1), actionElement.expander);
				}
			}
			else // => Deselected
			{
				// Remove all elements from action list (except "add action" button)
				MainWindow.window.actionsList.Children.RemoveRange(0, (MainWindow.window.actionsList.Children.Count - 1));
			}
		}

		public void AddAction(Action action = null)
		{
			// Create expander
			System.Windows.Controls.Expander expander = new System.Windows.Controls.Expander();
			expander.Header = "ACTION NAME";
			expander.Foreground = System.Windows.Media.Brushes.White;
			expander.Background = ColorHelper.ConvertHexToBrush("#3d3d3d");

			// Create border
			System.Windows.Controls.Border border = new System.Windows.Controls.Border();
			border.Padding = new Thickness(0, 0, 0, 10);

			// Create grid
			System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
			grid.Background = ColorHelper.ConvertHexToBrush("#3d3d3d");

			// Create label
			System.Windows.Controls.Label label = new System.Windows.Controls.Label();
			label.Content = "Action:";
			label.HorizontalAlignment = HorizontalAlignment.Left;
			label.VerticalAlignment = VerticalAlignment.Top;
			label.Margin = new Thickness(5, 7, 0, 0);
			label.Foreground = System.Windows.Media.Brushes.White;

			// Create comboBox (select action)
			System.Windows.Controls.ComboBox selectAction = new System.Windows.Controls.ComboBox();
			selectAction.HorizontalAlignment = HorizontalAlignment.Left;
			selectAction.VerticalAlignment = VerticalAlignment.Top;
			selectAction.Width = 534;
			selectAction.Margin = new Thickness(57, 10, 0, 0);

			int count = 0,
				selectActionIndex = -1;

			foreach (string actionName in ActionManager.GetActionNameList()) // Add all actions
			{
				// Select action in list when one is set
				if (action != null)
				{
					if ($"{ActionManager.actionNameStartWith} {action.GetActionName()}" == actionName)
					{
						selectActionIndex = count;
					}

					count++;
				}

				selectAction.Items.Add(actionName);
			}

			selectAction.SelectedIndex = selectActionIndex; // Select the set action if any or the default one (none)

			// Add to expander
			grid.Children.Add(label);
			grid.Children.Add(selectAction);
			border.Child = grid;
			expander.Content = border;

			// Add listener to comboBox (select action)
			selectAction.SelectionChanged += OnSelectedActionChanged;

			// Add to list of actions (as second last item)
			MainWindow.window.actionsList.Children.Insert((MainWindow.window.actionsList.Children.Count - 1), expander);

			// Add empty action
			ActionElement actionElement = new ActionElement();
			actionElement.expander = expander;
			actionElements.Add(actionElement);

			// Set header
			expander.Header = "No action selected";

			// Specific action is set (when loading a profile this will be the case)
			if (action != null)
			{
				SetAction(actionElement.expander, action);
			}
		}

		private void OnSelectedActionChanged(object sender, EventArgs e)
		{
			System.Windows.Controls.ComboBox selectAction = (System.Windows.Controls.ComboBox)sender;
			System.Windows.Controls.Grid selectActionGrid = (System.Windows.Controls.Grid)selectAction.Parent;
			System.Windows.Controls.Expander expander = (System.Windows.Controls.Expander)((System.Windows.Controls.Border)selectActionGrid.Parent).Parent;

			// Get name of selected action
			string selectedActionName = selectAction.SelectedItem.ToString().Remove(0, (ActionManager.actionNameStartWith.Length + 1));

			// Set action
			SetAction(expander, selectedActionName);
		}

		private void SetAction(System.Windows.Controls.Expander expander, string actionName)
		{
			System.Windows.Controls.Grid selectActionGrid = (System.Windows.Controls.Grid)((System.Windows.Controls.Border)expander.Content).Child;

			// Remove old action elements (except label and comboBox to select the action)
			selectActionGrid.Children.RemoveRange(2, (selectActionGrid.Children.Count - 1));

			// Find index of action "box"
			int actionIndex = MainWindow.window.actionsList.Children.IndexOf(expander);

			// Get action element by index
			ActionElement actionElement = actionElements.ElementAt(actionIndex);

			// Go through all action's types
			bool foundAction = false;

			foreach (Type actionType in ActionManager.GetActionTypesList())
			{
				// Create a new instance of the action with the action type
				Action action = (Action)Activator.CreateInstance(actionType);

				// If the action's name matches the searched name
				if (action.GetActionName() == actionName)
				{
					foundAction = true;
					actionElement.action = action;
					break;
				}
			}

			// No action found
			if (!foundAction)
			{
				actionElement.action = null;
				expander.Header = Action.NO_ACTION_NAME;
			}

			// Set header to name of action and add controls if any valid action is selected
			if (actionElement.action != null)
			{
				actionElement.action.AddControls(selectActionGrid);
				expander.Header = actionElement.action.GetHeaderName();
			}
		}

		private void SetAction(System.Windows.Controls.Expander expander, Action action)
		{
			System.Windows.Controls.Grid selectActionGrid = (System.Windows.Controls.Grid)((System.Windows.Controls.Border)expander.Content).Child;

			// Remove old action elements (except label and comboBox to select the action)
			selectActionGrid.Children.RemoveRange(2, (selectActionGrid.Children.Count - 1));

			// Find index of action "box"
			int actionIndex = MainWindow.window.actionsList.Children.IndexOf(expander);

			// Get action element by index
			ActionElement actionElement = actionElements.ElementAt(actionIndex);

			// Set action
			actionElement.action = action;

			// Add controls for action
			actionElement.action.AddControls(selectActionGrid);
			expander.Header = actionElement.action.GetHeaderName();
		}

		public bool HasActions()
		{
			return (actionElements.Count != -1);
		}

		public void ExecuteActions(bool isKeyDown)
		{
			Thread thread = new Thread(new ThreadStart(() =>
			{
				foreach (ActionElement actionElement in actionElements)
				{
					actionElement.action.Execute(isKeyDown);
				}
			}));

			thread.IsBackground = true;
			thread.Start();
		}

		public void RemoveAllActions()
		{
			// Remove all elements from action list (except "add action" button)
			MainWindow.window.actionsList.Children.RemoveRange(0, (MainWindow.window.actionsList.Children.Count - 1));

			// Set list to empty
			actionElements.RemoveRange(0, actionElements.Count);
		}
		
		public void SetText(string text, bool updateImage = true)
		{
			imageText = text;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetTextFontSize(int fontSize, bool updateImage = true)
		{
			textFontSize = fontSize;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetTextOffsetLeft(int offsetLeft, bool updateImage = true)
		{
			textOffsetLeft = offsetLeft;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetTextOffsetTop(int offsetTop, bool updateImage = true)
		{
			textOffsetTop = offsetTop;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetFontColor(System.Windows.Media.Color color, bool updateImage = true)
		{
			// Convert "Media.Color" to "Drawing.Brush"
			fontColor = ColorHelper.ConvertMediaColorToDrawingBrush(color);

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetFontColor(Brush brush, bool updateImage = true)
		{
			fontColor = brush;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetBackgroundColor(System.Windows.Media.Color color, bool updateImage = true)
		{
			// Convert "Media.Color" to "Drawing.Brush"
			backgroundColor = ColorHelper.ConvertMediaColorToDrawingBrush(color);

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void SetBackgroundColor(Brush brush, bool updateImage = true)
		{
			backgroundColor = brush;

			// Update image
			if (updateImage)
				SetImage(imagePath);
		}

		public void ResetToDefault()
		{
			// Remove all actions
			RemoveAllActions();

			// Reset attributes back to default
			imagePath = "";
			imageText = "";
			textFontSize = DEFAULT_TEXT_FONT_SIZE;
			textOffsetLeft = DEFAULT_TEXT_OFFSET_LEFT;
			textOffsetTop = DEFAULT_TEXT_OFFSET_TOP;
			fontColor = DEFAULT_FONT_COLOR;
			backgroundColor = DEFAULT_BACKGROUND_COLOR;

			// Set image to empty
			SetImage("");
		}

		public static DeckButton GetByWPF(System.Windows.Controls.Image image)
		{
			return buttons.Find(b => b.image == image);
		}

		public static DeckButton GetByStreamDeckKeyID(int streamDeckKeyID)
		{
			return buttons.Find(btn => btn.streamDeckKeyID == streamDeckKeyID);
		}

		public static DeckButton GetSelectedButton()
		{
			return selectedButton;
		}

		public static ActionElement GetActionElementByAction(Action action)
		{
			// Find and return action element
			return GetSelectedButton().actionElements.Find(ae => ae.action == action);
		}

		/* STREAM DECK */
		public void UpdateStreamDeckImage(Bitmap bitmap)
		{
			if (!StreamDeckHandler.IsStreamDeckConnected())
				return;

			// Create key bitmap from graphics
			using (var stream = new MemoryStream())
			{
				bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

				var keyImage = KeyBitmap.FromStream(stream);
				StreamDeckHandler.GetStreamDeck().SetKeyBitmap(streamDeckKeyID, keyImage);
			}
		}

		public static void SetAllToEmpty()
		{
			// Set all images to empty
			foreach (DeckButton button in buttons)
			{
				button.SetImage("");
			}
		}

		// CONVERT
		public BitmapImage ImageSourceFromBitmap(Bitmap src)
		{
			MemoryStream ms = new MemoryStream();
			src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			ms.Seek(0, SeekOrigin.Begin);
			image.StreamSource = ms;
			image.EndInit();
			return image;
		}
	}
}