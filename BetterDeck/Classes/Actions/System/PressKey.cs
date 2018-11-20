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
	public class PressKey : Action
	{
		private const string ACTION_NAME = "Press key";

		public bool holdDown = false;
		public VirtualKeyCode keyCode = VirtualKeyCode.NONAME;

		private ComboBox selectKeyComboBox;
		private CheckBox holdKeyDownCheckBox;

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

			if (keyCode != VirtualKeyCode.NONAME)
			{
				name += $" (_{KeyManager.GetKeyCodeName(keyCode)})";
			}

			return name;
		}

		public override void AddControls(Grid grid)
		{
			// Create label
			Label label = new Label();
			label.Content = "Simulate key:";
			label.HorizontalAlignment = HorizontalAlignment.Left;
			label.VerticalAlignment = VerticalAlignment.Top;
			label.Margin = new Thickness(5, 34, 0, 0);
			label.Foreground = Brushes.White;

			// Create "select key" combo box
			selectKeyComboBox = new ComboBox();
			selectKeyComboBox.HorizontalAlignment = HorizontalAlignment.Left;
			selectKeyComboBox.VerticalAlignment = VerticalAlignment.Top;
			selectKeyComboBox.Width = 501;
			selectKeyComboBox.Margin = new Thickness(90, 37, 0, 0);
			selectKeyComboBox.SelectionChanged += OnSelectedKeyChanged;

			int count = 0,
				selectIndex = 0;

			foreach (VirtualKeyCode keyCode in KeyManager.GetAllKeyCodes()) // Add all key codes
			{
				// Select key in list when one is set
				if (keyCode == this.keyCode)
				{
					selectIndex = count;
				}

				selectKeyComboBox.Items.Add(KeyManager.GetKeyCodeName(keyCode));

				count++;
			}

			selectKeyComboBox.SelectedIndex = selectIndex; // Select the set key or the default one (none)

			// Create checkBox
			holdKeyDownCheckBox = new CheckBox();
			holdKeyDownCheckBox.Content = "Hold key down (while pressed)";
			holdKeyDownCheckBox.IsChecked = holdDown;
			holdKeyDownCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
			holdKeyDownCheckBox.VerticalAlignment = VerticalAlignment.Top;
			holdKeyDownCheckBox.Height = 15;
			holdKeyDownCheckBox.Margin = new Thickness(9, 60, 0, 0);
			holdKeyDownCheckBox.Foreground = Brushes.White;
			holdKeyDownCheckBox.Checked += OnHoldDownKeyChecked;
			holdKeyDownCheckBox.Unchecked += OnHoldDownKeyUnchecked;

			// Add controls to grid
			grid.Children.Add(label);
			grid.Children.Add(selectKeyComboBox);
			grid.Children.Add(holdKeyDownCheckBox);
		}

		public override void Execute(bool isKeyDown)
		{
			InputSimulator input = new InputSimulator();

			if (holdDown) // Hold key down
			{
				if (isKeyDown) // Key is down
				{
					input.Keyboard.KeyDown(keyCode);
				}
				else // Key is up
				{
					input.Keyboard.KeyUp(keyCode);
				}
			}
			else if (isKeyDown) // Just press the key
			{
				input.Keyboard.KeyPress(keyCode);
			}
		}

		#endregion

		#region class specific methods

		public void SetKey(VirtualKeyCode keyCode)
		{
			this.keyCode = keyCode;

			// Update the header name
			UpdateHeaderName();
		}

		public void SetHoldKeyDown(bool holdDown)
		{
			this.holdDown = holdDown;
		}

		public VirtualKeyCode GetKeyCode()
		{
			return keyCode;
		}

		public void UpdateHoldKeyDownCheckbox()
		{
			holdKeyDownCheckBox.IsChecked = holdDown;
		}

		#endregion

		#region events

		private void OnSelectedKeyChanged(object sender, EventArgs e)
		{
			// Get selected key name
			string selectedKeyName = ((ComboBox)sender).SelectedItem.ToString();

			// Get key code by the selected name
			VirtualKeyCode keyCode = KeyManager.GetKeyCodeByName(selectedKeyName);

			// Set the key
			SetKey(keyCode);
		}

		private void OnHoldDownKeyChecked(object sender, EventArgs e)
		{
			SetHoldKeyDown(true);
		}

		private void OnHoldDownKeyUnchecked(object sender, EventArgs e)
		{
			SetHoldKeyDown(false);
		}

		#endregion
	}
}
