using ECAT.Core;
using System;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias
	{
		/// <summary>
		/// <see cref="ISignalDataInterpreter"/> for <see cref="CharacteristicValuesPowerSignal"/>
		/// </summary>
		private class CharacteristicValuesPowerSignalInterpreter : ISignalDataInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor, requires one parameter
			/// </summary>
			/// <param name="signal"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public CharacteristicValuesPowerSignalInterpreter(CharacteristicValuesPowerSignal signal)
			{
				_Signal = signal ?? throw new ArgumentNullException(nameof(signal));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// <see cref="CharacteristicValuesPowerSignal"/> to interpret
			/// </summary>
			private CharacteristicValuesPowerSignal _Signal { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Returns <see cref="double.NaN"/> (for <see cref="CharacteristicValuesPowerSignal"/> it's not possible to calculate RMS)
			/// </summary>
			/// <returns></returns>
			public double RMS() => double.NaN;

			/// <summary>
			/// Returns the average
			/// </summary>
			/// <returns></returns>
			double ISignalDataInterpreter.Average() => _Signal.Average;

			/// <summary>
			/// Returns the maximum
			/// </summary>
			/// <returns></returns>
			double ISignalDataInterpreter.Maximum() => _Signal.Maximum;

			/// <summary>
			/// Returns the minimum
			/// </summary>
			/// <returns></returns>
			double ISignalDataInterpreter.Minimum() => _Signal.Minimum;

			#endregion
		}
	}
}