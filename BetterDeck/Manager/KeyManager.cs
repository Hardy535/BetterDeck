using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsInput.Native;

namespace BetterDeck
{
	class KeyManager
	{
		private static List<VirtualKeyCode> keys = new List<VirtualKeyCode>();

		public static void InitKeyCodes()
		{
			// Add "no key"
			keys.Add(VirtualKeyCode.NONAME);

			// Add all available key codes
			Array keyCodes = Enum.GetValues(typeof(VirtualKeyCode));

			foreach (VirtualKeyCode keyCode in keyCodes)
			{
				if (keyCode == VirtualKeyCode.NONAME)
					continue;

				keys.Add(keyCode);
			}
		}

		public static List<VirtualKeyCode> GetAllKeyCodes()
		{
			return keys;
		}

		public static string GetKeyCodeName(VirtualKeyCode keyCode)
		{
			if (keyCode == VirtualKeyCode.NONAME) return ""; // Return empty string for "NONAME"
			else return keyCode.ToString();
		}

		public static VirtualKeyCode GetKeyCodeByName(string keyName)
		{
			return keys.Find(k => GetKeyCodeName(k) == keyName);
		}
	}
}
