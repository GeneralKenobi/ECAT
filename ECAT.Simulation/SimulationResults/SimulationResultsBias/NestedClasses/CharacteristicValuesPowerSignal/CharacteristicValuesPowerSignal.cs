using ECAT.Core;

namespace ECAT.Simulation
{
	public partial class SimulationResultsBias
	{
		/// <summary>
		/// Class for power signals that can't be fully constructed, instead only some characteristic values may be computed, for
		/// example maximum value.
		/// </summary>
		private class CharacteristicValuesPowerSignal : ISignalData
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public CharacteristicValuesPowerSignal()
			{
				Interpreter = new CharacteristicValuesPowerSignalInterpreter(this);
			}

			/// <summary>
			/// Constructor with parameters
			/// </summary>
			public CharacteristicValuesPowerSignal(double maximum, double minimum, double average) : this()
			{
				Maximum = maximum;
				Minimum = minimum;
				Average = average;
			}

			#endregion

			#region Public properties

			/// <summary>
			/// Unit to display
			/// </summary>
			public string Unit { get; } = IoC.Resolve<ISIUnits>().PowerShort;

			/// <summary>
			/// The maximum instantenous value
			/// </summary>
			public double Maximum { get; private set; }

			/// <summary>
			/// The minimum instantenous value
			/// </summary>
			public double Minimum { get; private set; }

			/// <summary>
			/// The average value
			/// </summary>
			public double Average { get; private set; }

			/// <summary>
			/// Interpreter of this <see cref="ISignalData"/>
			/// </summary>
			public ISignalDataInterpreter Interpreter { get; }

			#endregion
		}
	}
}