using System.Collections.Generic;
using System.Drawing;

namespace BetterDeck.Classes
{
	public class SaveDeckButton
	{
		public int streamDeckKeyID { get; set; }
		public string imagePath { get; set; }
		public string imageText { get; set; }
		public int textFontSize { get; set; }
		public int textOffsetLeft { get; set; }
		public int textOffsetTop { get; set; }
		public Brush fontColor { get; set; }
		public Brush backgroundColor { get; set; }
		public List<Action> actions { get; set; }

		public SaveDeckButton(int streamDeckKeyID, string imagePath, string imageText, int textFontSize, int textOffsetLeft, int textOffsetTop, Brush fontColor, Brush backgroundColor, List<Action> actions)
		{
			this.streamDeckKeyID = streamDeckKeyID;
			this.imagePath = imagePath;
			this.imageText = imageText;
			this.textFontSize = textFontSize;
			this.textOffsetLeft = textOffsetLeft;
			this.textOffsetTop = textOffsetTop;
			this.fontColor = fontColor;
			this.backgroundColor = backgroundColor;
			this.actions = actions;
		}
	}
}
