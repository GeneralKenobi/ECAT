using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ECAT.UWP
{
	/// <summary>
	/// Control for editing properties specific to an <see cref="IResistor"/>
	/// </summary>
	public sealed partial class ResistorEditUC : UserControl
    {
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ResistorEditUC()
		{
			this.InitializeComponent();
		}

		#endregion
	}
}