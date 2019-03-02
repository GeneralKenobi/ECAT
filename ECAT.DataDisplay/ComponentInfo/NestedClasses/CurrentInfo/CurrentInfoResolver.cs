using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Resolves currents on components
		/// </summary>
		private class CurrentInfoResolver : ResolverBase
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="targetType"></param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public CurrentInfoResolver(Type targetType) : base(targetType) { }

			#endregion			

			#region Public Methods

			/// <summary>
			/// Gets a current flow
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			protected override ISignalInformation GetSignalInformation(IBaseComponent target)
			{
				// Get currents
				var results = IoC.Resolve<ISimulationResultsProvider>().Value.Current;

				// The info to get
				ISignalInformation info = null;

				// Get the results from provider and return its return value (lazy cases because we operate on interfaces)
				TypeSwitch.Construct().
					// For resistors no change in VI directions means that current flow is taken for voltage from node A (reference) to node B
					LazyCase<IResistor>((x) => info = results.Get(x, !target.ChangeVIDirections)).
					// For capacitors no change in VI directions means that current flow is taken for voltage from node A (reference) to node B
					LazyCase<ICapacitor>((x) => info = results.Get(x, !target.ChangeVIDirections)).
					// For active components no change in VI directions means that current flow is taken for voltage from node A (reference) to node B
					LazyCase<IActiveComponent>((x) => info = results.Get(x.Index, !target.ChangeVIDirections)).
					Switch(target);

				return info;
			}

			#endregion
		}
	}
}