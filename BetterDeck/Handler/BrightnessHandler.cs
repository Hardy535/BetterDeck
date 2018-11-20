using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDeck.Handler
{
	public class BrightnessHandler
	{
		public const byte DEFAULT_BRIGHTNESS = 100;

		public static void SetBrightness(int brightness, bool updateSlider = true)
		{
			MainWindow.window.brightnessLabel.Content = brightness + "%";

			if (updateSlider)
				MainWindow.window.brightnessSlider.Value = brightness;

			// Set brightness on stream deck
			if (StreamDeckHandler.IsStreamDeckConnected())
			{
				StreamDeckHandler.GetStreamDeck().SetBrightness((byte)brightness);
			}
		}

		public static int GetBrightness()
		{
			return (int)MainWindow.window.brightnessSlider.Value;
		}
	}
}
