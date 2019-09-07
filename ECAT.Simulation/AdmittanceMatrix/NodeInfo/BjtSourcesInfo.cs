using ECAT.Core;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Contains information about sources in BJTs
	/// </summary>
	public class BjtSourcesInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor without parameters
		/// </summary>
		/// <param name="baseTerminal"></param>
		/// <param name="collectorTerminal"></param>
		/// <param name="emitterTerminal"></param>
		public BjtSourcesInfo() { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="baseTerminal"></param>
		/// <param name="collectorTerminal"></param>
		/// <param name="emitterTerminal"></param>
		public BjtSourcesInfo(IDCVoltageSource sourceBI, IDCVoltageSource sourceEI)
		{
			SourceBI = sourceBI;
			SourceEI = sourceEI;
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="baseTerminal"></param>
		/// <param name="collectorTerminal"></param>
		/// <param name="emitterTerminal"></param>
		public BjtSourcesInfo(IDCVoltageSource sourceBI, IDCVoltageSource sourceEI, IDCVoltageSource sourceCI)
		{
			SourceBI = sourceBI;
			SourceEI = sourceEI;
			SourceCI = sourceCI;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Source between Base and Inner terminals (used to measure base current)
		/// </summary>
		public IDCVoltageSource SourceBI { get; }

		/// <summary>
		/// Source between Emitter and Inner terminals (Base-Emitter voltage drop)
		/// </summary>
		public IDCVoltageSource SourceEI { get; }

		/// <summary>
		/// Source between Collector and Inner terminals (exists for saturated op-amps only)
		/// </summary>
		public IDCVoltageSource SourceCI { get; }

		/// <summary>
		/// Number of existing (not null) voltge sources
		/// </summary>
		public int Count => (SourceBI == null ? 0 : 1) + (SourceEI == null ? 0 : 1) + (SourceCI == null ? 0 : 1);

		/// <summary>
		/// Returns all non-null sources in a sequnce
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDCVoltageSource> AsEnumerable()
		{
			if(SourceBI != null)
			{
				yield return SourceBI;
			}

			if (SourceEI != null)
			{
				yield return SourceEI;
			}

			if (SourceCI != null)
			{
				yield return SourceCI;
			}
		}


		#endregion
	}
}
