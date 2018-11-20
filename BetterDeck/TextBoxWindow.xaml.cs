using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BetterDeck
{
	public partial class TextBoxWindow : Window
	{
		public string text = "";

		public TextBoxWindow()
		{
			InitializeComponent();
		}

		private void OnOkClicked(object sender, RoutedEventArgs e)
		{
			text = textBox.Text;
			Close();
		}

		private void OnCancelClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
