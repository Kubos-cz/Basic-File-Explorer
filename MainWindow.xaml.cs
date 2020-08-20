using System.Collections.ObjectModel;
using System.Windows;

namespace BaiscFileExplorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.DataContext = new AppVM();
		}
    }
}
