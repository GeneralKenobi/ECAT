using ECAT.Core;
using ECAT.Design;
using ECAT.ViewModel;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ECAT.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public MainPage()
		{
			this.InitializeComponent();

			// Assign data context
			DataContext = AppViewModel.Singleton;

			// Subscribe to events
			Loaded += MainPageLoaded;
			AppViewModel.Singleton.DesignVM.ComponentEditSectionVM.EditedPartChangedEvent += OnEditedComponentChanged;
		}

		#endregion

		#region Private Methods

		#region On loaded

		/// <summary>
		/// Adds the background grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainPageLoaded(object sender, RoutedEventArgs e)
		{
			// Add a horizontal and a vertical line every 25 pixels
			for (int i = 50; i < 2000; i += 50)
			{
				BackgroundGrid.Children.Add(new Line()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 2,
					X1 = 0,
					X2 = 2000,
					Y1 = i,
					Y2 = i,
				});

				BackgroundGrid.Children.Add(new Line()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 2,
					X1 = i,
					X2 = i,
					Y1 = 0,
					Y2 = 2000,
				});
			}
		}

		#endregion

		#region Main canvas tapped

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

		#region Drag event

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

		#region Edited component changed

		/// <summary>
		/// Handles changes in edited components (hides/shows side menu when necessary)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnEditedComponentChanged(object sender, EditedComponentChangedEventArgs args)
		{
			switch (args.Change)
			{
				case EditedComponentChanged.NullToComponent:
				case EditedComponentChanged.NoChange:
				case EditedComponentChanged.ComponentToComponent:
					{
						// Show the edit menu if it wasn't shown
						if (SideMenu.GetSelectedContentIndex() != 1)
						{
							SideMenu.SetSelectedContentFromIndex(1);
						}

						SideMenu.IsOpen = true;
					}
					break;

				case EditedComponentChanged.ComponentToNull:
					{
						// Hide the menu
						SideMenu.IsOpen = false;
					}
					break;
			}
		}

		#endregion

		#region Key pressed

		/// <summary>
		/// Handles key presses
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void KeyPressed(object sender, KeyRoutedEventArgs e)
		{
			if(e.Key == VirtualKey.Shift || e.Key == VirtualKey.Control || e.Key == VirtualKey.Menu)
			{
				return;
			}
			
			var shift = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down) ? KeyModifiers.Shift : KeyModifiers.None;
			var ctrl = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down) ? KeyModifiers.Ctrl : KeyModifiers.None;
			var alt = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down) ? KeyModifiers.Alt : KeyModifiers.None;
			AppViewModel.Singleton.ShortcutManager.ProcessKeyCombination(new ShortcutKey(e.Key.ToString(),
				shift | ctrl | alt));
		}

		#endregion

		#endregion
	}
}