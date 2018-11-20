using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDeck.Helper
{
	public class ColorHelper
	{
		public static Brush ConvertMediaColorToDrawingBrush(System.Windows.Media.Color color)
		{
			System.Windows.Media.Brush mediaBrush = new System.Windows.Media.SolidColorBrush(color);
			Brush brush = new SolidBrush((Color)new ColorConverter().ConvertFromString(new System.Windows.Media.BrushConverter().ConvertToString(color)));

			return brush;
		}

		public static System.Windows.Media.Color ConvertDrawingBrushToMediaColor(Brush brush)
		{
			Color brushColor = ((SolidBrush)brush).Color;
			System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(brushColor.A, brushColor.R, brushColor.G, brushColor.B);

			return mediaColor;
		}

		public static System.Windows.Media.SolidColorBrush ConvertHexToBrush(string hexColorCode)
		{
			return (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFrom(hexColorCode);
		}
	}
}
