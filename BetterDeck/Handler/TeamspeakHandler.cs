using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Net.Sockets;
using System.Threading;
using TS3QueryLib.Core;
using TS3QueryLib.Core.Client;
using TS3QueryLib.Core.Common.Responses;
using TS3QueryLib.Core.Communication;
using TS3QueryLib.Core.Client.Entities;

namespace BetterDeck.Handler
{
	public enum TeamspeakStatus
	{
		TBD, // TO BE DETERMINED
		NOT_REACHABLE,
		PROTOCOL_ERROR,
		AUTH_FAILED,
		CONNECTED
	}

	public class TeamspeakHandler
	{
		private const string API_KEY = "ENTER-HERE";
		private const string SERVER_ADDRESS = "localhost";
		private const ushort PORT = 25639;

		private static AsyncTcpDispatcher queryDispatcher;
		private static QueryRunner queryRunner;
		private static TeamspeakStatus teamspeakStatus = TeamspeakStatus.TBD;

		public static void CheckTeamspeakConnection()
		{
			while (true)
			{
				try
				{
					if (queryDispatcher == null || !queryDispatcher.IsConnected)
					{
						// Connect to localhost via TCP
						queryDispatcher = new AsyncTcpDispatcher(SERVER_ADDRESS, PORT);
						queryDispatcher.ReadyForSendingCommands += OnQueryDispatcherReadyForSendingCommands; // This event will be triggered when the client is ready to use commands
						queryDispatcher.ServerClosedConnection += OnQueryDispatcherClosedConnection;
						queryDispatcher.SocketError += OnQueryDispatcherSocketError;
						queryDispatcher.Connect();
					}

					Thread.Sleep(3000);
				}
				catch (ThreadInterruptedException ex)
				{
					return; // End while loop
				}
			}
		}

		private static void OnQueryDispatcherReadyForSendingCommands(object sender, EventArgs e)
		{
			// If query dispatcher is not connected (anymore)
			if (queryDispatcher == null)
			{
				Disconnect();
				return;
			}

			// Try to authenticate with the given API key
			queryRunner = new QueryRunner(queryDispatcher);
			SimpleResponse response = queryRunner.Authenticate(API_KEY);

			// Authentication was not successful
			if (response.IsErroneous)
			{
				SetTeamspeakStatus(TeamspeakStatus.AUTH_FAILED);
				return;
			}

			// Set teamspeak status
			SetTeamspeakStatus(TeamspeakStatus.CONNECTED);
		}

		private static void OnQueryDispatcherClosedConnection(object sender, EventArgs e)
		{
			// Set teamspeak status
			SetTeamspeakStatus(TeamspeakStatus.NOT_REACHABLE);

			// Reset
			Disconnect();
		}

		private static void OnQueryDispatcherSocketError(object sender, SocketErrorEventArgs e)
		{
			// Ignore "Connection reset" errors as they get handles by "OnQueryDispatcherConnectionClosed"
			if (e.SocketError == SocketError.ConnectionReset)
				return;

			switch (e.SocketError)
			{
				case SocketError.ConnectionRefused:
					{
						// Set teamspeak status
						SetTeamspeakStatus(TeamspeakStatus.NOT_REACHABLE);
						break;
					}
				default:
					{
						SetTeamspeakStatus(TeamspeakStatus.PROTOCOL_ERROR);
						//MessageBox.Show("Socket error: " + e.SocketError, "Teamspeak API", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
					}
			}

			// Reset
			Disconnect();
		}

		private static void SetTeamspeakStatus(TeamspeakStatus status)
		{
			// Do nothing if the status didn't change
			if (teamspeakStatus == status)
				return;

			teamspeakStatus = status;

			MainWindow.window.Dispatcher.Invoke(new Action(() =>
			{
				switch (status)
				{
					case TeamspeakStatus.NOT_REACHABLE:
						{
							MainWindow.window.teamspeakStatusGrid.ToolTip = "Can't connect to teamspeak!";
							MainWindow.window.teamspeakStatusEllipse.Fill = Brushes.Red;
							break;
						}
					case TeamspeakStatus.PROTOCOL_ERROR:
						{
							MainWindow.window.teamspeakStatusGrid.ToolTip = "Protocol error!";
							MainWindow.window.teamspeakStatusEllipse.Fill = Brushes.Red;
							break;
						}
					case TeamspeakStatus.AUTH_FAILED:
						{
							MainWindow.window.teamspeakStatusGrid.ToolTip = "Authentication failed (wrong API key)!";
							MainWindow.window.teamspeakStatusEllipse.Fill = Brushes.Red;
							break;
						}
					case TeamspeakStatus.CONNECTED:
						{
							MainWindow.window.teamspeakStatusGrid.ToolTip = "Connected";
							MainWindow.window.teamspeakStatusEllipse.Fill = Brushes.LimeGreen;
							break;
						}
				}
			}));
		}

		private static TeamspeakStatus GetTeamspeakStatus()
		{
			return teamspeakStatus;
		}

		private static void Disconnect()
		{
			// QueryRunner disposes the Dispatcher too
			if (queryRunner != null)
			{
				queryRunner.Dispose();
			}

			queryDispatcher = null;
			queryRunner = null;
		}

		/* PUBLIC METHODS */
		public static bool IsConnected()
		{
			if (queryDispatcher == null || queryRunner == null) return false;

			try
			{
				return (queryDispatcher.IsConnected && !queryDispatcher.IsDisposed && !queryRunner.IsDisposed && GetTeamspeakStatus() == TeamspeakStatus.CONNECTED &&
					queryRunner.GetChannelConnectionInfo().ErrorId == 0);
			}
			catch (NullReferenceException)
			{
				return false;
			}
		}

		public static void SetInputMuted(bool muted)
		{
			if (!IsConnected())
				return;

			ClientModification client = new ClientModification();
			client.IsInputMuted = muted;

			try
			{
				queryRunner.UpdateClient(client);
			}
			catch (NullReferenceException)
			{
				return;
			}
		}

		public static void SetOutputMuted(bool muted)
		{
			if (!IsConnected())
				return;

			ClientModification client = new ClientModification();
			client.IsOutputMuted = muted;

			try
			{
				queryRunner.UpdateClient(client);
			}
			catch (NullReferenceException)
			{
				return;
			}
		}
	}
}







// TEAMSPEAK TEST
/*const string serverAddress = "localhost";
const ushort port = 25639;

queryDispatcher = new AsyncTcpDispatcher(serverAddress, port);
queryDispatcher.Connect();

queryRunner = new QueryRunner(queryDispatcher);
queryRunner.Authenticate("YKFD-DOWL-S5UP-GVV6-0XM5-DWYQ");

TS3QueryLib.Core.Client.Entities.ClientModification client = new TS3QueryLib.Core.Client.Entities.ClientModification();
client.IsInputMuted = true;

queryRunner.UpdateClient(client);*/
