using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Resolves power on components
		/// </summary>
		private class PowerInfoResolver : ResolverBase
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="targetType"></param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public PowerInfoResolver(Type targetType) : base(targetType) { }

			#endregion			

			#region Public Methods

			/// <summary>
			/// Gets a power on a component
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			protected override ISignalInformation GetSignalInformation(IBaseComponent target)
			{
				// Get currents
				var results = IoC.Resolve<ISimulationResultsProvider>().Value.Power;

				// The info to get
				ISignalInformation info = null;

				// Get the results from provider and return its return value (lazy cases because we operate on interfaces)
				TypeSwitch.Construct().
					LazyCase<IResistor>((x) => info = results.Get(x)).
					LazyCase<ICurrentSource>((x) => info = results.Get(x)).
					// Because IACVoltageSource extens IVoltageSource the check for that needs to be done manually so as not to
					// fetch the result twice (first only for IVoltageSource then for IACVoltageSource)
					LazyCase<IVoltageSource>((x) => info = x is IACVoltageSource xAC ? results.Get(xAC) : results.Get(x)).
					Switch(target);

				return info;
			}

			#endregion
		}
	}
}