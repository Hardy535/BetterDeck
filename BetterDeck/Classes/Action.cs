using BetterDeck.Classes.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BetterDeck.Classes
{
	public abstract class Action
	{
		public const string NO_ACTION_NAME = "No action selected";

		public abstract ActionType GetActionType();
		public abstract string GetActionName();
		public abstract string GetHeaderName();
		public abstract void AddControls(Grid grid);
		public abstract void Execute(bool isKeyDown);

		public void UpdateHeaderName()
		{
			// Find action element
			ActionElement actionElement = DeckButton.GetActionElementByAction(this);

			// Set header name
			actionElement.expander.Header = GetHeaderName();
		}
	}
}
