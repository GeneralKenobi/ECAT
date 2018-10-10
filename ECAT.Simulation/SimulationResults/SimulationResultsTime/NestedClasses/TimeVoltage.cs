using ECAT.Core;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	partial class SimulationResultsTime
	{
		/// <summary>
		/// Manages voltage-related results
		/// </summary>
		private class TimeVoltage : VoltageCache<ITimeDomainSignal, ITimeDomainSignal>, IVoltageDB
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="nodePotentials">Sequence of pairs, key is a node, values are potentials calculated
			/// for time instants for that node</param>
			/// <param name="startTime">Start time of the simulation</param>
			/// <param name="timeStep">Time step of the simulation - difference between two subsequent simulation points</param>
			/// <exception cref="ArgumentNullException"></exception>
			public TimeVoltage(IEnumerable<KeyValuePair<INode, ITimeDomainSignal>> data, double timeStep, double startTime) : base(data)
			{
				_TimeStep = timeStep;
				_StartTime = startTime;
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Time step of the simulation - difference between two subsequent simulation points
			/// </summary>
			private double _TimeStep { get; }

			/// <summary>
			/// Start time of the simulation
			/// </summary>
			private double _StartTime { get; }

			#endregion

			#region Protected methods

			/// <summary>
			/// Constructs a new <see cref="PhasorDomainSignal"/> based on voltage drop between two nodes (with <paramref name="nodeA"/>
			/// being the reference node). Caches the result (with its negation). Node indexes are assumed to have been checked that
			/// corresponding to them nodes exist in <see cref="_Nodes"/>, if not an exception may be thrown.
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				// Get the nodes
				var nodeA = _Data.First((node) => node.Key.Index == nodeAIndex);
				var nodeB = _Data.First((node) => node.Key.Index == nodeBIndex);

				var nodeAEnum = nodeA.Value.InstantenousValues.GetEnumerator();
				var nodeBEnum = nodeB.Value.InstantenousValues.GetEnumerator();

				List<double> voltageDropValues = new List<double>();

				while(nodeAEnum.MoveNext() && nodeBEnum.MoveNext())
				{
					voltageDropValues.Add(nodeBEnum.Current - nodeAEnum.Current);
				}


				// Construct the result
				return IoC.Resolve<ITimeDomainSignal>(voltageDropValues, _TimeStep, _StartTime);					
			}

			/// <summary>
			/// Returns a negated (voltage drop direction is reversed) copy of <paramref name="signal"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <returns></returns>
			protected override ITimeDomainSignal CopyAndNegate(ITimeDomainSignal signal) => signal.CopyAndNegate();

			#endregion

			#region Public methods

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns null if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <param name="nodeToGround">If true, voltage drop is calculated from ground to node given by
			/// <paramref name="nodeIndex"/>, if false it is calculated from node given by <paramref name="nodeIndex"/> to ground</param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeIndex, bool nodeToGround = true)
			{
				return null;
			}

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns null
			/// if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformation Get(int nodeAIndex, int nodeBIndex)
			{
				return null;
			}

			/// <summary>
			/// Gets information on voltage drop across a <see cref="ITwoTerminal"/> component
			/// </summary>
			/// <param name="component"></param>
			/// <param name="voltageBA">If true, voltage drop is calculated from <see cref="ITwoTerminal.TerminalB"/> to
			/// <returns></returns>
			public ISignalInformation Get(ITwoTerminal component, bool voltageBA = true)
			{
				return null;
			}

			#endregion
		}
	}
}