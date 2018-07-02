using ECAT.Core;
using ECAT.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ECAT.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			this.InitializeComponent();
			Loaded += MainPageLoaded;
			DataContext = AppViewModel.Singleton;
        }

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
				MainCanvas.Children.Add(new Path()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 1,
					Data = (Geometry)XamlReader.Load(
					"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
					"M0," + i.ToString() + " h2000</Geometry>"),
				});

				MainCanvas.Children.Add(new Path()
				{
					Stroke = App.Current.Resources["LightGrayBrush"] as SolidColorBrush,
					StrokeThickness = 1,
					Data = (Geometry)XamlReader.Load(
					"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
					"M" + i.ToString() + ",0 v2000</Geometry>"),
				});
			}
		}

		#endregion
		
		#region Parts added/deleted

		private void PartsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (var item in e.NewItems)
						{
							if (item is Wire wire)
							{
								//MainCanvas.Children.Add(new WireControl()
								//{
								//	DataContext = wire,
								//	Tag = wire.ID,
								//});
							}
							//else if (item is BasePart part)
							//{
							//	MainCanvas.Children.Add(new BasePartControl()
							//	{
							//		DataContext = item,
							//		Tag = part.ID,
							//	});
							//}
						}
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{
						foreach (var item in e.OldItems)
						{
							//if (item is BasePart part)
							//{
							//	foreach (var child in MainCanvas.Children)
							//	{
							//		if (child is FrameworkElement element &&
							//			element.Tag != null && (int)element.Tag == part.ID)
							//		{
							//			MainCanvas.Children.Remove(child);
							//			break;
							//		}
							//	}
							//}
						}
					}
					break;
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
		private void MainCanvasTapped(object sender, TappedRoutedEventArgs e)
		{
			var pointerCoord = e.GetPosition(sender as Canvas);

			//AppViewModel.Singleton.MainPanelClickedCommand.Execute(new Position(pointerCoord.X, pointerCoord.Y));
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

		#endregion
	}
}