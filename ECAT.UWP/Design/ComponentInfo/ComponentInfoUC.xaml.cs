using Windows.UI.Xaml.Controls;

namespace ECAT.UWP
{
	/// <summary>
	/// Control displaying the component info in the bottom left corner
	/// </summary>
	public sealed partial class ComponentInfoUC : UserControl
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ComponentInfoUC()
		{
			this.InitializeComponent();
			this.DataContextChanged += ComponentInfoUC_DataContextChanged;
		}

		private void ComponentInfoUC_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
		{
			
		}

		#endregion
	}
}