using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IOpAmp"/>s
	/// </summary>
	public class JfetEditViewModel : SpecificComponentEditViewModel<IJfet>
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public JfetEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public properties
		
		/// <summary>
		/// Gate
		/// </summary>
		public double RGS
		{
			get => _EditedComponent.RGS;
			set
			{
				if(value > 0)
				{
					_EditedComponent.RGS = value;
				}

				InvokePropertyChanged(nameof(RGS));
			}
		}

		/// <summary>
		/// Small-signal output resistance
		/// </summary>
		public double RDS
		{
			get => _EditedComponent.RDS;
			set
			{
				if (value > 0)
				{
					_EditedComponent.RDS = value;
				}

				InvokePropertyChanged(nameof(RDS));
			}
		}

		/// <summary>
		/// Transconductance
		/// </summary>
		public double GM
		{
			get => _EditedComponent.GM;
			set
			{
				if (value >= 0)
				{
					_EditedComponent.GM = value;
				}

				InvokePropertyChanged(nameof(GM));
			}
		}

		#endregion
	}
}