using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ECAT
{
	public sealed partial class ComponentAddingMenu : UserControl, INotifyPropertyChanged
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ComponentAddingMenu()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Property Changed Event

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private Members

		/// <summary>
		/// Index selected in the list with input parts
		/// </summary>
		private int mInputListSelectedIndex = -1;

		/// <summary>
		/// Index selected in the list with simple logic parts
		/// </summary>
		private int mSimpleLogicListSelectedIndex = -1;

		/// <summary>
		/// Index selected in the list with complex logic parts
		/// </summary>
		private int mComplexLogicListSelectedIndex = -1;

		#endregion

		#region Public Properties

		/// <summary>
		/// Index selected in the list with input parts
		/// </summary>
		public int InputListSelectedIndex
		{
			get => mInputListSelectedIndex;
			set
			{
				// Set the value
				mInputListSelectedIndex = value;

				// Uncheck the other lists
				mSimpleLogicListSelectedIndex = -1;
				mComplexLogicListSelectedIndex = -1;

				// Call Property Changed Event
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimpleLogicListSelectedIndex)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComplexLogicListSelectedIndex)));
			}
		}

		/// <summary>
		/// Index selected in the list with simple logic parts
		/// </summary>
		public int SimpleLogicListSelectedIndex
		{
			get => mSimpleLogicListSelectedIndex;
			set
			{
				// Set the value
				mSimpleLogicListSelectedIndex = value;

				// Uncheck the other lists
				mInputListSelectedIndex = -1;
				mComplexLogicListSelectedIndex = -1;

				// Call Property Changed Event
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputListSelectedIndex)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComplexLogicListSelectedIndex)));
			}
		}

		/// <summary>
		/// Index selected in the list with complex logic parts
		/// </summary>
		public int ComplexLogicListSelectedIndex
		{
			get => mComplexLogicListSelectedIndex;
			set
			{
				// Set the value
				mComplexLogicListSelectedIndex = value;

				// Uncheck the other lists
				mInputListSelectedIndex = -1;
				mSimpleLogicListSelectedIndex = -1;

				// Call Property Changed Event
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputListSelectedIndex)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimpleLogicListSelectedIndex)));
			}
		}


		#endregion

		#region Private Methods

		/// <summary>
		/// If the clicked item was selected deselects it. Normally it has to be done by ctrl+click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void PartAddingListViewItemClicked(object sender, ItemClickEventArgs e)
		{
			// If the sender is a listview and the clicked item is the selected item
			if (sender is ListView listView && e.ClickedItem == listView.SelectedItem)
			{
				// Wait for a bit because otherwise the item will be reselected
				await Task.Delay(25);

				// Remove the reference to the selected item
				listView.SelectedItem = null;
			}
		}

		#endregion
	}
}
