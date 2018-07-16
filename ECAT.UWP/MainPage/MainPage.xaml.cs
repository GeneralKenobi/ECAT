using ECAT.Core;
using ECAT.Design;
using ECAT.Simulation;
using ECAT.ViewModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ECAT.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public MainPage()
		{
			this.InitializeComponent();
			Loaded += MainPageLoaded;
			DataContext = AppViewModel.Singleton;
		}

		#endregion

		#region On Loaded

		/// <summary>
		/// Adds the background grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainPageLoaded(object sender, RoutedEventArgs e)
		{
			// Add a horizontal and a vertical line every 25 pixels
			for (int i = 25; i < 2000; i += 25)
			{
				BackgroundGrid.Children.Add(new Line()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 1,
					X1 = 0,
					X2 = 2000,
					Y1 = i,
					Y2 = i,
				});

				BackgroundGrid.Children.Add(new Line()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 1,
					X1 = i,
					X2 = i,
					Y1 = 0,
					Y2 = 2000,
				});
			}
		}

		#endregion

		#region Main Canvas Tapped

		/// <summary>
		/// Manages 'Tapped' events on the main canvas (invokes the
		/// MainPanelClickedCommand from the view model)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DesignAreaTapped(object sender, TappedRoutedEventArgs e)
		{
			var pointerCoord = e.GetPosition(sender as UIElement);

			AppViewModel.Singleton.DesignVM.DesignAreaClickedCommand.Execute(new PlanePosition(pointerCoord.X, -pointerCoord.Y));
		}

		#endregion

		#region Drag Event

		/// <summary>
		/// Handles drops that occur on the MainCanvas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DropOnMainCanvas(object sender, DragEventArgs e)
		{
			//// If the position before drag was found in the IoC
			//if (IoC.Get("PositionBeforeDrag", out Position pos))
			//{
			//	// Remove it
			//	IoC.Remove("PositionBeforeDrag");

			//	// Get the drop position relative to the MainCanvas
			//	var dropPosition = e.GetPosition(MainCanvas);

			//	// Assign the new position for the coord
			//	pos.Absolute.Set(dropPosition.X + pos.Shift.X, dropPosition.Y + pos.Shift.Y);

			//	e.Handled = true;
			//}
		}

		/// <summary>
		/// Configures a drag over event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PartDragOver(object sender, DragEventArgs e)
		{
			// Set the action to move
			e.AcceptedOperation = DataPackageOperation.Move;

			// Don't show the glyph
			e.DragUIOverride.IsGlyphVisible = false;

			// Set the caption to "Move"
			e.DragUIOverride.IsCaptionVisible = false;

			e.DragUIOverride.IsContentVisible = true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles changes in edited parts (hides/shows side menu when necessary)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnEditedPartChanged(object sender, /*EditedPartChangedEventArgs*/object args)
		{
			//switch (args.ChangeType)
			//{
			//	case EditedPartChangeType.NullToPart:
			//	case EditedPartChangeType.TheSame:
			//	case EditedPartChangeType.PartToPart:
			//		{
			//			// Swaps to the edit menu if it wasn't shown
			//			if (SideMenu.GetSelectedContentIndex() != 2)
			//			{
			//				SideMenu.SetSelectedContentFromIndex(2);
			//			}

			//			SideMenu.IsOpen = true;
			//		}
			//		break;

			//	case EditedPartChangeType.PartToNull:
			//		{
			//			// Hides the menu
			//			SideMenu.IsOpen = false;
			//		}
			//		break;
			//}
		}

		/// <summary>
		/// Handles key presses
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void KeyPressed(object sender, KeyRoutedEventArgs e)
		{
			if(e.Key == VirtualKey.Escape)
			{
				AppViewModel.Singleton.DesignVM.StopActionCommand.Execute(null);
			}
			if (e.Key == VirtualKey.W)
			{
				AppViewModel.Singleton.DesignVM.PrepareToPlaceLooseWireCommand.Execute(null);
			}
			if (e.Key == VirtualKey.R)
			{
				AppViewModel.Singleton.DesignVM.ComponentToAdd = new ComponentDeclaration(0, "Resistor", 2, ComponentType.Passive);
			}
			if (e.Key == VirtualKey.V)
			{
				AppViewModel.Singleton.DesignVM.ComponentToAdd = new ComponentDeclaration(1, "Voltage Source", 2, ComponentType.Passive);
			}
			if (e.Key == VirtualKey.C)
			{
				AppViewModel.Singleton.DesignVM.ComponentToAdd = new ComponentDeclaration(2, "Current Source", 2, ComponentType.Passive);
			}
		}

		#endregion
	}
}