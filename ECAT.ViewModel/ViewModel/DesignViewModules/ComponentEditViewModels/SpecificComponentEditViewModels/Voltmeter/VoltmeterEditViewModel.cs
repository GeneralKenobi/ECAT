using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for <see cref="IVoltmeter"/>
	/// </summary>
	public class VoltmeterEditViewModel : SpecificComponentEditViewModel<IVoltmeter>
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltmeterEditViewModel(ComponentViewModel componentViewModel) : base(componentViewModel) { }

		#endregion
	}
}
