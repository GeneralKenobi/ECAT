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
					LazyCase<IResistor>((x) => info = results.Get(x, false)).
					LazyCase<ICapacitor>((x) => info = results.Get(x, false)).
					LazyCase<IActiveComponent>((x) => info = results.Get(x.ActiveComponentIndex, false)).
					Switch(target);

				return info;
			}

			#endregion
		}
	}
}