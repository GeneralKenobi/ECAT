using System;
using System.ComponentModel;
using UWPEnhanced.Xaml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	/// <summary>
	/// Base for templated controls that want to have a border present based on visibility of some FlyoutContext
	/// </summary>
	public class BorderWithFlyoutMenuBaseTC : Control, INotifyPropertyChanged
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public BorderWithFlyoutMenuBaseTC(string contextMenuOwnerName, Brush borderPresentBrush = null)
		{
			_ContextMenuOwnerName = contextMenuOwnerName;

			_BorderColor = borderPresentBrush ?? App.Current.Resources["RedBrush"] as Brush;

			this.Loaded += OnLoaded;
		}

		#endregion		

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private properties

		/// <summary>
		/// The name of the control that owns the context menu
		/// </summary>
		private string _ContextMenuOwnerName { get; }

		/// <summary>
		/// The <see cref="Brush"/> to return when border should be present
		/// </summary>
		private Brush _BorderColor { get; }

		#endregion

		#region Public properties

		/// <summary>
		/// When true, the border around the part is shown
		/// </summary>
		public bool ShowBorder { get; private set; }

		/// <summary>
		/// SolidColorBrush to bind the BorderBrush to (
		/// </summary>
		public Brush MenuPresentBorderBrush => ShowBorder ?	_BorderColor : new SolidColorBrush(Colors.Transparent);

		#endregion

		#region Private methods

		/// <summary>
		/// Invoked when the control loads, assigns event handlers to events of the flyout menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this.TryFindChild(out var contextMenuOwner, _ContextMenuOwnerName) && contextMenuOwner.ContextFlyout != null)
			{
				contextMenuOwner.ContextFlyout.Opening += MenuFlyoutOpenClose;
				contextMenuOwner.ContextFlyout.Closed += MenuFlyoutOpenClose;
			}
			else
			{
				throw new Exception("Can't find child with name " + _ContextMenuOwnerName + " or it has no ContextFlyout defined");
			}
		}

		/// <summary>
		/// Toggles the <see cref="ShowBorder"/> flag
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuFlyoutOpenClose(object sender, object e) =>
			ShowBorder = !ShowBorder;

		#endregion
	}
}