using BetterDeck.Classes;
using BetterDeck.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetterDeck
{
	public class ImageTabHandler
	{
		public static void Update()
		{
			DeckButton selectedButton = DeckButton.GetSelectedButton();

			if (selectedButton == null)
				return;

			// Set image path
			MainWindow.window.imagePath.Text = selectedButton.imagePath;

			// Set text
			MainWindow.window.imageText.Text = selectedButton.imageText;

			// Set text font size
			MainWindow.window.imageTextFontSize.Value = selectedButton.textFontSize;

			// Set text offset left
			MainWindow.window.imageTextOffsetLeft.Value = selectedButton.textOffsetLeft;

			// Set text offset top
			MainWindow.window.imageTextOffsetTop.Value = selectedButton.textOffsetTop;

			// Set font color
			MainWindow.window.imageFontColor.SelectedColor = ColorHelper.ConvertDrawingBrushToMediaColor(selectedButton.fontColor);

			// Set background color
			MainWindow.window.imageBackgroundColor.SelectedColor = ColorHelper.ConvertDrawingBrushToMediaColor(selectedButton.backgroundColor);
		}
	}
}
