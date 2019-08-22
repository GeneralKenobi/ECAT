using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Class containing various helper methods for <see cref="DesignViewModel"/>
	/// </summary>
	internal static class DesignViewModelHelpers
    {
		#region Private static properties

		/// <summary>
		/// Dictionary listing types of view models for different components
		/// </summary>
		private static Dictionary<ComponentIDEnumeration, Type> _SpecificComponentEditViewModels { get; } =
			new Dictionary<ComponentIDEnumeration, Type>()
		{
			{ ComponentIDEnumeration.Resistor, typeof(ResistorEditViewModel) },
			{ ComponentIDEnumeration.VoltageSource, typeof(VoltageSourceEditViewModel) },
			{ ComponentIDEnumeration.CurrentSource, typeof(CurrentSourceEditViewModel) },
			{ ComponentIDEnumeration.Ground, typeof(SpecificComponentEditViewModel<IGround>) },
			{ ComponentIDEnumeration.OpAmp, typeof(OpAmpEditViewModel) },
			{ ComponentIDEnumeration.Capacitor, typeof(CapacitorEditViewModel) },
			{ ComponentIDEnumeration.ACVoltageSource, typeof(ACVoltageSourceEditViewModel) },
			{ ComponentIDEnumeration.SweepVoltageSource, typeof(SweepVoltageSourceEditViewModel) },
			{ ComponentIDEnumeration.Voltmeter, typeof(VoltmeterEditViewModel) },
			{ ComponentIDEnumeration.Inductor, typeof(InductorEditViewModel) },
			{ ComponentIDEnumeration.NpnBjt, typeof(BjtEditViewModel) },
		};

		#endregion

		#region Public static methods

		/// <summary>
		/// Constructs a MenuEditViewModel compatible with the passed <see cref="ComponentViewModel"/> and its
		/// <see cref="IBaseComponent"/>, if no match was found throws an exception
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		public static BaseComponentEditViewModel ConstructAppropriateEditViewModel(ComponentViewModel viewModel)
		{
			// Get component declaration for the view model's component and use it to search the dictionary
			if (_SpecificComponentEditViewModels.TryGetValue(IoC.Resolve<IComponentFactory>().GetDeclaration(
				viewModel.Component.GetType()).ID, out Type parameter))
			{				
				// Use activator to create an instance				
				return Activator.CreateInstance(parameter, viewModel) as BaseComponentEditViewModel;
			}

			throw new ArgumentException("Couldn't match a view model for the view model");
		}

		#endregion
	}
}