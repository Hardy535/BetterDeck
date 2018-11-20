using BetterDeck.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsInput;
using WindowsInput.Native;

namespace BetterDeck.Classes.Actions.System
{
	public class MuteInput : Action
	{
		private const string ACTION_NAME = "(Un)Mute input";

		public bool setMuted = false;

		private CheckBox setMutedCheckbox;

		#region override abstract methods

		public override ActionType GetActionType()
		{
			return ActionType.Teamspeak;
		}

		public override string GetActionName()
		{
			return ACTION_NAME;
		}

		public override string GetHeaderName()
		{
			string name = ACTION_NAME;

			if (setMuted) name += " (mute)";
			else name += " (unmute)";

			return name;
		}

		public override void AddControls(Grid grid)
		{
			// Create checkBox
			setMutedCheckbox = new CheckBox();
			setMutedCheckbox.Content = "Set muted (checked = muted | unchecked = unmuted)";
			setMutedCheckbox.IsChecked = setMuted;
			setMutedCheckbox.HorizontalAlignment = HorizontalAlignment.Left;
			setMutedCheckbox.VerticalAlignment = VerticalAlignment.Top;
			setMutedCheckbox.Height = 15;
			setMutedCheckbox.Margin = new Thickness(9, 40, 0, 0);
			setMutedCheckbox.Foreground = Brushes.White;
			setMutedCheckbox.Checked += OnSetMutedChecked;
			setMutedCheckbox.Unchecked += OnSetMutedUnchecked;

			// Add controls to grid
			grid.Children.Add(setMutedCheckbox);
		}

		public override void Execute(bool isKeyDown)
		{
			if (!isKeyDown)
				return;

			TeamspeakHandler.SetInputMuted(setMuted);
		}

		#endregion

		#region class specific methods

		public void SetMuted(bool muted)
		{
			this.setMuted = muted;

			// Update the header name
			UpdateHeaderName();
		}

		#endregion

		#region events

		private void OnSetMutedChecked(object sender, EventArgs e)
		{
			SetMuted(true);
		}

		private void OnSetMutedUnchecked(object sender, EventArgs e)
		{
			SetMuted(false);
		}

		#endregion
	}
}
