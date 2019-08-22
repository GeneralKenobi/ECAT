using ECAT.Core;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IOpAmp"/>s
	/// </summary>
	public class BjtEditViewModel : SpecificComponentEditViewModel<IBjt>
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
		public double Y11
		{
			get => _EditedComponent.Y11;
			set
			{
				if (value > 0)
				{
					_EditedComponent.Y11 = value;
					InvokePropertyChanged(nameof(Y11));
				}
			}
		}

		/// <summary>
		/// Reverse-transfer admittance
		/// </summary>
		public double Y12
		{
			get => _EditedComponent.Y12;
			set
			{
				if (value > 0)
				{
					_EditedComponent.Y12 = value;
					InvokePropertyChanged(nameof(Y12));
				}
			}
		}

		/// <summary>
		/// Forward-transfer admittance
		/// </summary>
		public double Y21
		{
			get => _EditedComponent.Y21;
			set
			{
				if (value > 0)
				{
					_EditedComponent.Y21 = value;
					InvokePropertyChanged(nameof(Y21));
				}
			}
		}

		/// <summary>
		/// Output admittance
		/// </summary>
		public double Y22
		{
			get => _EditedComponent.Y22;
			set
			{
				if (value > 0)
				{
					_EditedComponent.Y22 = value;
					InvokePropertyChanged(nameof(Y22));
				}
			}
		}
		
		/// <summary>
		/// Cutoff base-emitter voltage
		/// </summary>
		public double UBECutoff
		{
			get => _EditedComponent.UBECutoff;
			set => _EditedComponent.UBECutoff = value;
		}

		/// <summary>
		/// Saturation collector-emitter voltage
		/// </summary>
		public double UCESaturation
		{
			get => _EditedComponent.UCESaturation;
			set => _EditedComponent.UCESaturation = value;
		}

		#endregion
	}
}