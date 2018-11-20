using BetterDeck.Classes;
using BetterDeck.Classes.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDeck.Manager
{
	public class ActionManager
	{
		private static List<string> actionNamesList;
		private static List<System.Type> actionTypesList;

		private static string typeNameStartWith = ">";
		private static string typeNameEndWith = "<";
		public static string actionNameStartWith = "-";

		public static void InitActionNamesList()
		{
			// Add all action type names to the list
			actionNamesList = new List<string>();
			actionTypesList = new List<System.Type>();

			foreach (ActionType actionType in System.Enum.GetValues(typeof(ActionType)))
			{
				actionNamesList.Add($"{typeNameStartWith} {actionType.ToString()} {typeNameEndWith}");
			}
		}

		public static void RegisterAction(Action action)
		{
			// Insert action under the correct action type
			string actionTypeName = $"{typeNameStartWith} {action.GetActionType().ToString()} {typeNameEndWith}";
			int actionTypeIndex = actionNamesList.FindIndex(n => n == actionTypeName);

			actionNamesList.Insert((actionTypeIndex + 1), $"{actionNameStartWith} {action.GetActionName()}");

			// Add to action list
			actionTypesList.Add(action.GetType());
		}

		public static List<string> GetActionNameList()
		{
			return actionNamesList;
		}

		public static List<System.Type> GetActionTypesList()
		{
			return actionTypesList;
		}
	}
}
