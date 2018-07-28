﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPEnhanced.Xaml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ECAT.UWP
{
	public sealed class ComponentWrapperTC : BorderWithFlyoutMenuBaseTC
	{
		#region Constructor
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public ComponentWrapperTC() : base("RootGrid")
		{
			this.DefaultStyleKey = typeof(ComponentWrapperTC);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Adds the center coord
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PartDragStarting(UIElement sender, DragStartingEventArgs args)
		{
			//// Get the position of the click relative to the dragged control
			//var startPosition = args.GetPosition(sender);

			//// Try to cast the sender's DataContext to BasePart
			//var castedSender = (sender as UserControl).DataContext as BasePart;

			//// If we couldn't, cancel the drag event
			//if (castedSender == null)
			//{
			//	args.Cancel = true;
			//}

			//Position pos = new Position();

			//// Get the center coord of the dragged part
			//pos.Absolute = castedSender.CenterCoord;

			//// Calculate the shift applied during drop (because parts can be dragged not only be center but on the whole area)
			//// It's half of dimensions minus relative click position
			//pos.Shift.X = castedSender.Width / 2 - startPosition.X;
			//pos.Shift.Y = castedSender.Height / 2 - startPosition.Y;

			//// Store these 2 values in the IoC
			//try
			//{
			//	// RESERVE_IOC PositionBeforeDrag
			//	IoC.Add(pos, "PositionBeforeDrag");
			//}
			//catch (Exception)
			//{
			//	// If it wasn't possible
			//	if (IoC.Remove("PositionBeforeDrag"))
			//	{
			//		// And it was due to the fact that IoC already had entries under these names (which are now removed)
			//		try
			//		{
			//			// Try to add them once more
			//			IoC.Add(pos, "PositionBeforeDrag");
			//		}
			//		catch (Exception)
			//		{
			//			// If the operation failed again, cancel the drag event
			//			args.Cancel = true;
			//		}
			//	}
			//}

			//args.DragUI.SetContentFromBitmapImage(new BitmapImage(new Uri("ms-appx:///Assets/Pictures/MoveObjectPicture.png"))
			//{
			//	DecodePixelHeight = 28,
			//	DecodePixelWidth = 28,
			//});

		}

		#endregion
	}
}