using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for state of a circuit represented by waveforms
	/// </summary>
	public class WaveformState : GenericState<IEnumerable<double>>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		public WaveformState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices) : base(nodeIndices, activeComponentsIndices) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will are given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		public WaveformState(IEnumerable<int> nodeIndices, int activeComponentsCount) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount)) { }

		#endregion
	}
}