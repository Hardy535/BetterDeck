using StreamDeckSharp;
using StreamDeckSharp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace BetterDeck.Handler
{
	public enum StreamDeckStatus
	{
		TBD, // TO BE DETERMINED
		NOT_CONNECTED,
		CONNECTED
	}

	public class StreamDeckHandler
	{
		private static IStreamDeck _streamDeck = null;
		private static StreamDeckStatus _streamDeckStatus = StreamDeckStatus.TBD;

		public static IStreamDeck GetStreamDeck()
		{
			return _streamDeck;
		}

		public static bool IsStreamDeckConnected()
		{
			if (_streamDeck == null)
				return false;

			return _streamDeck.IsConnected;
		}

		public static void CheckStreamDeckConnection()
		{
			while (true)
			{
				try
				{
					// Stream deck is not connected
					if (!IsStreamDeckConnected())
					{
						try
						{
							// Try to find stream deck and establish connection
							_streamDeck = StreamDeck.OpenDevice();

							// Reset to default
							_streamDeck.ClearKeys();
							_streamDeck.KeyStateChanged += StreamDeckEventHandler.OnKeyStateChange;

							MainWindow.window.Dispatcher.Invoke(new Action(() =>
							{
								BrightnessHandler.SetBrightness(BrightnessHandler.DEFAULT_BRIGHTNESS);
							}));

							// Set stream deck status
							SetStreamDeckStatus(StreamDeckStatus.CONNECTED);
						}
						catch (StreamDeckNotFoundException ex)
						{
							// Stream deck was not found
							SetStreamDeckStatus(StreamDeckStatus.NOT_CONNECTED);
						}
					}

					Thread.Sleep(3000);
				}
				catch (ThreadInterruptedException ex2)
				{
					return; // End while loop
				}
				catch (TaskCanceledException ex2)
				{
					return; // End while loop
				}
			}
		}

		private static void SetStreamDeckStatus(StreamDeckStatus status)
		{
			// Do nothing if the status didn't change
			if (_streamDeckStatus == status)
				return;

			MainWindow.window.Dispatcher.Invoke(new Action(() =>
			{
				switch (status)
				{
					case StreamDeckStatus.NOT_CONNECTED:
						{
							MainWindow.window.streamDeckStatusGrid.ToolTip = "Not connected!";
							MainWindow.window.streamDeckStatusEllipse.Fill = Brushes.Red;
							break;
						}
					case StreamDeckStatus.CONNECTED:
						{
							MainWindow.window.streamDeckStatusGrid.ToolTip = "Connected";
							MainWindow.window.streamDeckStatusEllipse.Fill = Brushes.LimeGreen;
							break;
						}
				}
			}));
		}

		public static void Dispose()
		{
			_streamDeck.Dispose();
		}
	}
}
