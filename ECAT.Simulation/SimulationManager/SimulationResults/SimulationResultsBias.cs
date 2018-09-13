using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	public partial class SimulationManager
	{
		/// <summary>
		/// Standard implementation of <see cref="ISimulationManager"/>, manages results for <see cref="SimulationManager"/>
		/// </summary>
		private class SimulationResultsBias : ISimulationResults
		{
			#region Private properties

			/// <summary>
			/// List with all nodes upon which specific results are calculated
			/// </summary>
			private List<INode> _Nodes { get; set; } = new List<INode>();

			/// <summary>
			/// Cache with currents produced by <see cref="IVoltageSource"/>s, <see cref="IACVoltageSource"/>s and <see cref="IOpAmp"/>s
			/// </summary>
			private Dictionary<int, Tuple<IPhasorDomainSignal, SignalInformationNew>> _ActiveComponentsCurrentCache { get; set; } =
				new Dictionary<int, Tuple<IPhasorDomainSignal, SignalInformationNew>>();

			/// <summary>
			/// Dictionary holding already computed voltage drops for the last performed simulation. Ints in key tuple are indexes of
			/// nodes (Item1 for the first node (reference node) and Item2 for the second node (target node)). First item in value
			/// is the voltage drop, second one is an <see cref="SignalInformationNew"/> built based on Item1.
			/// </summary>
			private Dictionary<Tuple<int, int>, Tuple<PhasorDomainSignal, SignalInformationNew>> _VoltageDropCache { get; } =
				new Dictionary<Tuple<int, int>, Tuple<PhasorDomainSignal, SignalInformationNew>>(
					new CustomEqualityComparer<Tuple<int, int>>(
					// Compare the elements of the Tuples, now tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			/// <summary>
			/// Contains already computed currents, Item1 is in the standard direction, Item2 is in the reverse direction
			/// </summary>
			private Dictionary<IBaseComponent, Tuple<SignalInformationNew, SignalInformationNew>> _CurrentCache { get; } =
				new Dictionary<IBaseComponent, Tuple<SignalInformationNew, SignalInformationNew>>();

			/// <summary>
			/// Contains already computed <see cref="PowerInformation"/>s
			/// </summary>
			private Dictionary<IBaseComponent, PowerInformation> _PowerCache { get; } =
				new Dictionary<IBaseComponent, PowerInformation>();

			#endregion

			#region Private methods

			#region Caching

			/// <summary>
			/// Caches the <paramref name="signal"/> as well as its copy with inverted indexes into <see cref="_VoltageDropInformationCache"/>
			/// </summary>
			/// <param name="signal"></param>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			private void CacheVoltageDrop(PhasorDomainSignal signal, int nodeAIndex, int nodeBIndex)
			{
				// Cache the original
				_VoltageDropCache.Add(new Tuple<int, int>(nodeAIndex, nodeBIndex),
					new Tuple<PhasorDomainSignal, SignalInformationNew>(signal, new SignalInformationNew(signal)));				

				// And cache the reversed one
				var reversed = signal.CopyAndNegate();

				_VoltageDropCache.Add(new Tuple<int, int>(nodeBIndex, nodeAIndex), new Tuple<PhasorDomainSignal, SignalInformationNew>(
					reversed, new SignalInformationNew(reversed)));
			}

			/// <summary>
			/// Caches the <paramref name="current"/> in <see cref="_CurrentCache"/> as well as its negation
			/// </summary>
			/// <param name="component">Component for which the current flow is considered</param>
			/// <param name="current"></param>
			private void CacheCurrent(IBaseComponent component, PhasorDomainSignal current) =>				
				_CurrentCache.Add(component, new Tuple<SignalInformationNew, SignalInformationNew>(
					new SignalInformationNew(current), new SignalInformationNew(current.CopyAndNegate())));

			/// <summary>
			/// Caches the <paramref name="power"/> in <see cref="_PowerCache"/>			
			/// </summary>
			/// <param name="component">Component for which the current flow is considered</param>
			/// <param name="power"></param>
			private void CachePower(IBaseComponent component, PowerInformation power) =>
				_PowerCache.Add(component, power);

			/// <summary>
			/// Clears all caches of old values
			/// </summary>
			private void ClearCaches()
			{
				_VoltageDropCache.Clear();
				_ActiveComponentsCurrentCache.Clear();
				_CurrentCache.Clear();
				_PowerCache.Clear();
			}

			#endregion

			#region Voltage drop related

			/// <summary>
			/// Finds all AC voltage waveforms between the two node potentials collections
			/// </summary>
			/// <param name="nodeAACPotentials"></param>
			/// <param name="nodeBACPotentials"></param>
			/// <returns></returns>
			private IEnumerable<KeyValuePair<double, Complex>> GetACWaveforms(IDictionary<double, Complex> nodeAACPotentials,
				IDictionary<double, Complex> nodeBACPotentials)
			{
				// Get the intersecting keys (i.e. find all waveforms that are present at both nodes, in 90% situations it will be all
				// elements but not always)
				var intersectingKeys = nodeAACPotentials.Keys.Intersect(nodeBACPotentials.Keys);

				// For each waveform present at both nodes
				foreach (var key in intersectingKeys)
				{
					// Return a difference between waveform at node B and waveform at node A
					yield return new KeyValuePair<double, Complex>(key, nodeBACPotentials[key] - nodeAACPotentials[key]);
				}

				// For each waveform present only at node B
				foreach (var key in nodeBACPotentials.Keys.Except(intersectingKeys))
				{
					// Add its value to the waveforms
					yield return new KeyValuePair<double, Complex>(key, nodeBACPotentials[key]);
				}

				// For each waveform present only at node A
				foreach (var key in nodeAACPotentials.Keys.Except(intersectingKeys))
				{
					// Subtract its value from the waveforms
					yield return new KeyValuePair<double, Complex>(key, -nodeAACPotentials[key]);
				}
			}

			/// <summary>
			/// Checks if <see cref="ISignalInformation.Maximum"/> is a positive value, if not inverts the direction of voltage
			/// drops and sets the <see cref="ISignalInformation.InvertedDirection"/> flag to true
			/// </summary>
			/// <param name="info"></param>
			private PhasorDomainSignal GetForPositiveMaximum(PhasorDomainSignal info) => info.DC >= 0 ? info : info.CopyAndNegate();

			/// <summary>
			/// Constructs a new VoltageDropInformation based on two nodes (with <paramref name="nodeA"/> being the reference node)
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			private PhasorDomainSignal ConstructVoltageDrop(INode nodeA, INode nodeB) => GetForPositiveMaximum(
				new PhasorDomainSignal(nodeB.DCPotential.Value - nodeA.DCPotential.Value,
					GetACWaveforms(nodeA.ACPotentials, nodeB.ACPotentials)));

			/// <summary>
			/// Constructs a new <see cref="PhasorDomainSignal"/> based on node indexes. If at least one of the indexes is not
			/// corresponding to any of the nodes in <see cref="_Nodes"/>, returns a <see cref="PhasorDomainSignal"/> equal to 0.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			private PhasorDomainSignal ConstructVoltageDrop(int nodeAIndex, int nodeBIndex) => 
				NodeExists(nodeAIndex) && NodeExists(nodeBIndex) ? 	ConstructVoltageDrop(
				_Nodes.Find((node) => node.Index == nodeAIndex), _Nodes.Find((node) => node.Index == nodeBIndex)) :
				new PhasorDomainSignal();
			
			/// <summary>
			/// Returns voltage drop between nodes A (reference) and B. If at least one node index isn't related to any of the nodes
			/// in <see cref="_Nodes"/>, returns a voltage drop equal to 0. If there already was a cached value, return it.
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			private PhasorDomainSignal ResolveVoltageDrop(int nodeAIndex, int nodeBIndex)
			{
				PhasorDomainSignal result = null;

				// Check the cache
				if (_VoltageDropCache.TryGetValue(new Tuple<int, int>(nodeAIndex, nodeBIndex), out var voltageDrop))
				{
					result = voltageDrop.Item1;
				}
				else
				{
					// If not successful, try to construct voltage drop (check if nodes exist first)
					result = NodeExists(nodeAIndex) && NodeExists(nodeBIndex) ?
						ConstructVoltageDrop(nodeAIndex, nodeBIndex) : new PhasorDomainSignal();
				}

				return result;
			}

			#endregion

			#region Current Related

			/// <summary>
			/// Returns a DC current flowing through a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private double GetPassiveTwoTerminalDCCurrent(IPhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.DC * twoTerminal.GetConductance();

			/// <summary>
			/// Returns AC current phasors for a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private IEnumerable<KeyValuePair<double, Complex>> GetPassiveTwoTerminalACCurrentPhasors(
				PhasorDomainSignal voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.ComposingPhasors.Select((phasor) =>
				new KeyValuePair<double, Complex>(phasor.Key, phasor.Value * twoTerminal.GetAdmittance(phasor.Key)));

			/// <summary>
			/// Returns current information about a standard two terminal element. Designed for: <see cref="IResistor"/>,
			/// <see cref="ICapacitor"/>
			/// </summary>
			/// <param name="element">Element for which the current is considered</param>
			/// <param name="reverseDirection">Reverses the direction upon which current flow is decided</param>
			/// <returns></returns>
			private ISignalInformationNew GetStandardPassiveTwoTerminalCurrent(ITwoTerminal element, bool reverseDirection)
			{
				// If there was a cache entry already return it
				if(_CurrentCache.TryGetValue(element, out var current))
				{
					return reverseDirection ? current.Item2 : current.Item1;
				}

				// Get voltage drop across the element
				var voltageDrop = reverseDirection ?
					ResolveVoltageDrop(element.TerminalA.NodeIndex, element.TerminalB.NodeIndex) :
					ResolveVoltageDrop(element.TerminalB.NodeIndex, element.TerminalA.NodeIndex);

				// Create a new current signal
				var currentSignal = new PhasorDomainSignal()
				{
					DC = GetPassiveTwoTerminalDCCurrent(voltageDrop, element),
					ComposingPhasors = GetPassiveTwoTerminalACCurrentPhasors(voltageDrop, element),
				};

				// Cache it
				CacheCurrent(element, currentSignal);
				
				return new SignalInformationNew(currentSignal);
			}

			#endregion

			#region General

			/// <summary>
			/// Returns true if a node with the given index exists
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			private bool NodeExists(int index) => _Nodes.Exists((node) => node.Index == index);

			#endregion

			#endregion

			#region Public methods

			#region Data loading

			/// <summary>
			/// Loads new nodes based on which results are computed
			/// </summary>
			/// <param name="nodes"></param>
			public void LoadNewData(IEnumerable<INode> nodes, IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> vsCurrents)
			{
				// Clear the old, already computed entries
				ClearCaches();

				// Create a new list with nodes
				_Nodes = new List<INode>(nodes);

				// Creates a dictionary of active component currents
				_ActiveComponentsCurrentCache =
					new Dictionary<int, Tuple<IPhasorDomainSignal, SignalInformationNew>>(vsCurrents.ToDictionary(
					(current) => current.Key, (current) => new Tuple<IPhasorDomainSignal, SignalInformationNew>(
						current.Value, new SignalInformationNew(current.Value))));
						
				// Add an empty node as the ground node (which is normally not included in simulation due to optimization)
				// and effectively increment every node index by 1
				_Nodes.Insert(0, new Node() { Index = GroundNodeIndex });
			}

			#endregion

			#region Voltage drop related

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <returns></returns>
			public bool TryGetVoltageDrop(int nodeIndex, out ISignalInformationNew voltageDrop) =>
				TryGetVoltageDrop(GroundNodeIndex, nodeIndex, out voltageDrop);

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node). If the node
			/// indexes exceed currently held nodes count null is assigned to <paramref name="voltageDrop"/> and false is returned
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public bool TryGetVoltageDrop(int nodeAIndex, int nodeBIndex, out ISignalInformationNew voltageDrop)
			{
				// If one of the node indexes exceeds the number of nodes
				if(nodeAIndex >= _Nodes.Count || nodeBIndex >= _Nodes.Count)
				{
					voltageDrop = null;
					return false;
				}

				// If the node indexes are equal (the same nodes) return default voltage drop (equivalent to no drop)
				if(nodeAIndex == nodeBIndex)
				{
					voltageDrop = new SignalInformationNew();
					return true;
				}

				// If that particular voltage drop was determined already return it
				if(_VoltageDropCache.TryGetValue(new Tuple<int, int>(nodeAIndex, nodeBIndex), out var cachedVoltageDrop))
				{
					voltageDrop = cachedVoltageDrop.Item2;
					return true;
				}

				// Otherwise construct it
				var signal = ConstructVoltageDrop(_Nodes[nodeAIndex], _Nodes[nodeBIndex]);

				// Cache it
				CacheVoltageDrop(signal, nodeAIndex, nodeBIndex);

				voltageDrop = new SignalInformationNew(signal);

				// And return success
				return true;
			}

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns a drop equal to zero if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <returns></returns>
			public ISignalInformationNew GetVoltageDropOrZero(int nodeIndex) => GetVoltageDropOrZero(GroundNodeIndex, nodeIndex);				

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns a drop
			/// equal to zero if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformationNew GetVoltageDropOrZero(int nodeAIndex, int nodeBIndex)
			{
				if(TryGetVoltageDrop(nodeAIndex, nodeBIndex, out var info))
				{
					return info;
				}
				else
				{
					return new SignalInformationNew();
				}
			}

			#endregion

			#region Current related

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="reverseDirection">Reverses the direction upon which current flow is decided</param>
			/// <returns></returns>
			public ISignalInformationNew GetCurrent(IResistor resistor, bool reverseDirection) =>
				GetStandardPassiveTwoTerminalCurrent(resistor, reverseDirection);

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="reverseDirection">Reverses the direction upon which current flow is decided</param>
			/// <returns></returns>
			public ISignalInformationNew GetCurrent(ICapacitor capacitor, bool reverseDirection) =>
				GetStandardPassiveTwoTerminalCurrent(capacitor, reverseDirection);

			/// <summary>
			/// Returns current produced by some <see cref="IActiveComponent"/>
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
			/// by convention for the specific element</param>
			/// <returns></returns>
			public ISignalInformationNew GetCurrentOrZero(int activeComponentIndex, bool reverseDirection)
			{
				// If the current can be found
				if(_ActiveComponentsCurrentCache.TryGetValue(activeComponentIndex, out var signal))
				{
					// If reversion was requested
					if(reverseDirection)
					{
						// Make a copy
						//signal = new SignalInformation(signal);
						// And negate it
						//NegateSignal(signal);
					}

					return signal.Item2;
				}

				return new SignalInformationNew();
			}

			#endregion

			#region Power related

			/// <summary>
			/// Gets information about power dissipated on an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(ISignalInformation voltageDrop, IResistor resistor)
			{
				// Check if there already is a cached entry
				if(_PowerCache.TryGetValue(resistor, out var power))
				{
					return power;
				}

				// If not create a new power info
				var result = new PowerInformation()
				{
					// Average power on a resistor is a sqaure of DC voltage plus half of squares of AC magnitudes (RMS values) times
					// the conductance of the resistor
					Average = (voltageDrop.ComposingPhasors.Sum((phasor) => Math.Pow(phasor.Value.Magnitude, 2)) / 2 +
					Math.Pow(voltageDrop.DC, 2)) * resistor.GetConductance(),

					// Maximum occurs for maximum voltage drop and is simply a square of voltage times conductance
					Maximum = Math.Pow(voltageDrop.Maximum, 2) * resistor.GetConductance(),
				};

				// Cache it
				CachePower(resistor, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(ICurrentSource currentSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(currentSource, out var power))
				{
					return power;
				}

				var voltageDrop = ResolveVoltageDrop(currentSource.TerminalB.NodeIndex, currentSource.TerminalA.NodeIndex);

				// Average is negative voltage drop times produced current (to abide passive sign convention)
				var result = new PowerInformation()
				{
					Average = -voltageDrop.DC * currentSource.ProducedCurrent,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)
				// It's the minimum voltage drop minus twice DC voltage drop times current. (Minimum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Minimum = (voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				// It's the maximum voltage drop minus twice DC voltage drop times current. (Maximum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Maximum = (voltageDrop.Interpreter.Maximum() - 2 * voltageDrop.DC) * currentSource.ProducedCurrent;

				// Cache the calculated value
				CachePower(currentSource, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="IVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(IVoltageSource voltageSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(voltageSource, out var power))
				{
					return power;
				}

				if(!_ActiveComponentsCurrentCache.TryGetValue(voltageSource.ActiveComponentIndex, out var currentPackage))
				{
					return new PowerInformation();
				}

				var current = currentPackage.Item1;

				// Average is voltage drop times produced current (current is assumed to flow right to left in standard convention,
				// current produced flows left to right so produced power is negative)
				var result = new PowerInformation()
				{
					Average = -current.DC * voltageSource.ProducedDCVoltage,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)				
				result.Minimum = current.Interpreter.Minimum() * voltageSource.ProducedDCVoltage;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				result.Maximum = current.Interpreter.Maximum() * voltageSource.ProducedDCVoltage;

				// Cache the calculated value
				CachePower(voltageSource, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="IACVoltageSource"/>. If the <paramref name="current"/> is composed of
			/// phasors with different frequency than that of <paramref name="voltageSource"/> the average power will be assigned
			/// <see cref="Double.NaN"/> (it's impossible to calculate it using only phasors). Doesn't compute maximum/minimum
			/// instantenous power.
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(IACVoltageSource voltageSource)
			{
				// TODO: When time-based simulation is implemented try to calcuate the average iteratively if there is more than one
				// current phasor - for example as an average of power calculated for n points in one full cycle

				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(voltageSource, out var power))
				{
					return power;
				}

				if (!_ActiveComponentsCurrentCache.TryGetValue(voltageSource.ActiveComponentIndex, out var currentPackage))
				{
					return new PowerInformation();
				}

				var current = currentPackage.Item1;

				// Create a new info, initialize Minimum and Maximum with NaN to indicate they couldn't have been calculated
				var result = new PowerInformation()
				{
					Minimum = double.NaN,
					Maximum = double.NaN,
				};

				// If it has only ony AC
				if (current.Type.HasFlag(SignalType.AC))
				{
					// If there is more than one phasor or the phasor, for some reason, has a different frequency than the source
					if (current.Type.HasFlag(SignalType.MultipleAC) || current.ComposingPhasors.First().Key != voltageSource.Frequency)
					{
						// Assign NaN as the average value cannot be easily computed
						result.Average = double.NaN;
					}
					else
					{
						// Get the only phasor composing the 
						var singlePhasors = current.ComposingPhasors.First();

						// Calculate the average as Vrms*Irms*cos(phiV - phiI)
						// TODO: When IAsyncVoltageSource has phase shift, include it in the formula
						result.Average = current.Interpreter.RMS() * Math.Sqrt(Math.Pow(voltageSource.ProducedDCVoltage, 2) +
							Math.Pow(voltageSource.PeakProducedVoltage, 2) / 2) * Math.Cos(singlePhasors.Value.Phase);
					}
				}

				// TODO: Try to think of a way to calculate max/min instantenous power of the source

				// Cache the calculated value
				CachePower(voltageSource, result);

				// And return it
				return result;
			}

			#endregion

			#endregion
		}
	}
}