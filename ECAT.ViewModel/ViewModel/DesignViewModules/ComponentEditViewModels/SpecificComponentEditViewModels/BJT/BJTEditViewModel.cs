using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IOpAmp"/>s
	/// </summary>
	public class BjtEditViewModel : TransistorEditViewModel<IBjt>
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="componentViewModel"></param>
		public BjtEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion

		#region Public properties

		/// <summary>
		/// Input impedance
		/// </summary>
		public double H11
		{
			get => _EditedComponent.H11;
			set
			{
				if (value > 0)
				{
					_EditedComponent.H11 = value;
					InvokePropertyChanged(nameof(H11));
				}
			}
		}

		/// <summary>
		/// Reverse-voltage feedback
		/// </summary>
		public double H12
		{
			get => _EditedComponent.H12;
			set
			{
				if (value > 0)
				{
					_EditedComponent.H12 = value;
					InvokePropertyChanged(nameof(H12));
				}
			}
		}

		/// <summary>
		/// Forward current gain
		/// </summary>
		public double H21
		{
			get => _EditedComponent.H21;
			set
			{
				if (value > 0)
				{
					_EditedComponent.H21 = value;
					InvokePropertyChanged(nameof(H21));
				}
			}
		}

		/// <summary>
		/// Output admittance
		/// </summary>
		public double H22
		{
			get => _EditedComponent.H22;
			set
			{
				if (value > 0)
				{
					_EditedComponent.H22 = value;
					InvokePropertyChanged(nameof(H22));
				}
			}
		}
		
		/// <summary>
		/// Cutoff base-emitter voltage
		/// </summary>
		public double UBEForward
		{
			get => _EditedComponent.UBEForward;
			set => _EditedComponent.UBEForward = value;
		}

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		public double UCESaturation
		{
			get => _EditedComponent.UCESaturation;
			set => _EditedComponent.UCESaturation = value;
		}

		/// <summary>
		/// Beta coefficient of the BJT
		/// </summary>
		public double Beta
		{
			get => _EditedComponent.Beta;
			set => _EditedComponent.Beta = value;
		}

		#endregion
	}
}