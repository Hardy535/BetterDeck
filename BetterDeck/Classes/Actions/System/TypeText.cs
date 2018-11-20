using BetterDeck.Helper;
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
	public class TypeText : Action
	{
		private const string ACTION_NAME = "Type text";

		public string text = "";
		public bool hitEnter = false;

		private CheckBox hitEnterCheckBox;

		#region override abstract methods

		public override ActionType GetActionType()
		{
			return ActionType.System;
		}

		public override string GetActionName()
		{
			return ACTION_NAME;
		}

		public override string GetHeaderName()
		{
			string name = ACTION_NAME;

			if (text != null && text.Length > 0)
			{
				string showText = text; // Create copy to modify (if necessary)

				if (showText.Length > 49)
				{
					showText = text.Remove(49);
					showText += "...";
				}

				name += $" ({showText})";
			}

			return name;
		}

		public override void AddControls(Grid grid)
		{
			// Create label
			Label label = new Label();
			label.Content = "Type text:";
			label.HorizontalAlignment = HorizontalAlignment.Left;
			label.VerticalAlignment = VerticalAlignment.Top;
			label.Margin = new Thickness(5, 34, 0, 0);
			label.Foreground = Brushes.White;

			// Create textBox
			TextBox textBox = new TextBox();
			textBox.Text = text;
			textBox.HorizontalAlignment = HorizontalAlignment.Left;
			textBox.VerticalAlignment = VerticalAlignment.Top;
			textBox.Width = 520;
			textBox.Margin = new Thickness(71, 38, 0, 0);
			textBox.Foreground = Brushes.White;
			textBox.Background = ColorHelper.ConvertHexToBrush("#565360");
			textBox.TextChanged += OnTextChanged;

			// Create checkBox
			hitEnterCheckBox = new CheckBox();
			hitEnterCheckBox.Content = "Hit enter when typed";
			hitEnterCheckBox.IsChecked = hitEnter;
			hitEnterCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
			hitEnterCheckBox.VerticalAlignment = VerticalAlignment.Top;
			hitEnterCheckBox.Height = 15;
			hitEnterCheckBox.Margin = new Thickness(9, 60, 0, 0);
			hitEnterCheckBox.Foreground = Brushes.White;
			hitEnterCheckBox.Checked += OnHitEnterWhenTypedChecked;
			hitEnterCheckBox.Unchecked += OnHitEnterWhenTypedUnhecked;

			// Add controls to grid
			grid.Children.Add(label);
			grid.Children.Add(textBox);
			grid.Children.Add(hitEnterCheckBox);
		}

		public override void Execute(bool isKeyDown)
		{
			InputSimulator input = new InputSimulator();

			if (isKeyDown)
			{
				input.Keyboard.TextEntry(text);

				if (hitEnter)
				{
					input.Keyboard.KeyPress(VirtualKeyCode.RETURN);
				}
			}
		}

		#endregion

		#region class specific methods

		public void SetText(string text)
		{
			this.text = text;

			// Update the header name
			UpdateHeaderName();
		}

		public void SetHitEnter(bool hitEnter)
		{
			this.hitEnter = hitEnter;
		}

		public void UpdateHitEnterCheckbox()
		{
			hitEnterCheckBox.IsChecked = hitEnter;
		}

		#endregion

		#region events

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			SetText(((TextBox)sender).Text);
		}

		private void OnHitEnterWhenTypedChecked(object sender, EventArgs e)
		{
			SetHitEnter(true);
		}

		private void OnHitEnterWhenTypedUnhecked(object sender, EventArgs e)
		{
			SetHitEnter(false);
		}

		#endregion
	}
}
