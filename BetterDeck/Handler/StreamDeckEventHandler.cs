using BetterDeck.Classes;
using BetterDeck.Classes.Actions;
using StreamDeckSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetterDeck.Handler
{
	public class StreamDeckEventHandler
	{
		public static void OnKeyStateChange(object sender, KeyEventArgs e)
		{
			// Key can't be found
			if (DeckButton.GetByStreamDeckKeyID(e.Key) == null)
			{
				MessageBox.Show("There was an error determining the key you pressed on your stream deck!\n" +
					"Please contact the developers with any information you can get about this issue on GitHub.", "Better Deck");
				return;
			}

			// Execute actions for this key
			DeckButton.GetByStreamDeckKeyID(e.Key).ExecuteActions(e.IsDown);
		}
	}
}
