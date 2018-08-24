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
			private Dictionary<int, SignalInformation> _ActiveComponentsCurrentCache { get; set; } =
				new Dictionary<int, SignalInformation>();

			/// <summary>
			/// Dictionary holding already computed voltage drops for the last performed simulation. Ints in key tuple are indexes of
			/// nodes (Item1 for the first node (reference node) and Item2 for the second node (target node))
			/// </summary>
			private Dictionary<Tuple<int, int>, SignalInformation> _VoltageDropCache { get; } =
				new Dictionary<Tuple<int, int>, SignalInformation>(new CustomEqualityComparer<Tuple<int, int>>(
					// Compare the elements of the Tuples, now tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			/// <summary>
			/// Cache for currents, first item in key tuple is component for which the current is considered and second item is
			/// the voltage drop for which the current flow is considered.
			/// </summary>
			private Dictionary<Tuple<IBaseComponent, ISignalInformation>, SignalInformation> _CurrentCache { get; } =
				new Dictionary<Tuple<IBaseComponent, ISignalInformation>, SignalInformation>(
					new CustomEqualityComparer<Tuple<IBaseComponent, ISignalInformation>>(
					// Compare the elements of the Tuples, now tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			/// <summary>
			/// Cache for power, first item in key tuple is component for which the power is considered and second item is
			/// the voltage drop for which the power is considered.
			/// </summary>
			private Dictionary<Tuple<IBaseComponent, ISignalInformation>, PowerInformation> _PowerCache { get; } =
				new Dictionary<Tuple<IBaseComponent, ISignalInformation>, PowerInformation>(
					new CustomEqualityComparer<Tuple<IBaseComponent, ISignalInformation>>(
					// Compare the elements of the Tuples, now tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			#endregion

			#region Private methods

			#region Caching

			/// <summary>
			/// Caches the <paramref name="info"/> as well as its copy with inverted indexes into <see cref="_VoltageDropCache"/>
			/// </summary>
			/// <param name="info"></param>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			private void CacheVoltageDrop(SignalInformation info, int nodeAIndex, int nodeBIndex)
			{
				// Get a copy for opposite node mapping
				var copy = info.CopySignalInformation();

				// Negate it
				NegateSignal(copy);

				// Cache the original
				_VoltageDropCache.Add(new Tuple<int, int>(nodeAIndex, nodeBIndex), info);

				// And cache the copy
				_VoltageDropCache.Add(new Tuple<int, int>(nodeBIndex, nodeAIndex), copy);
			}

			/// <summary>
			/// Caches the <paramref name="current"/> in <see cref="_CurrentCache"/>
			/// </summary>
			/// <param name="component">Component for which the current flow is considered</param>
			/// <param name="voltageDrop">Voltage drop on the component for which the current is considered</param>
			/// <param name="current"></param>
			private void CacheCurrent(IBaseComponent component, ISignalInformation voltageDrop, SignalInformation current) =>				
				_CurrentCache.Add(new Tuple<IBaseComponent, ISignalInformation>(component, voltageDrop), current);

			/// <summary>
			/// Caches the <paramref name="power"/> in <see cref="_PowerCache"/>			
			/// </summary>
			/// <param name="component">Component for which the current flow is considered</param>
			/// <param name="voltageDrop">Voltage drop on the component for which the current is considered</param>
			/// <param name="power"></param>
			private void CachePower(IBaseComponent component, ISignalInformation voltageDrop, PowerInformation power) =>
				_PowerCache.Add(new Tuple<IBaseComponent, ISignalInformation>(component, voltageDrop), power);

			/// <summary>
			/// Clears all caches of old values
			/// </summary>
			private void ClearCaches()
			{
				_VoltageDropCache.Clear();
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
			/// Calculates and assigns characteristic voltages - maximum, minimum, RMS
			/// </summary>
			/// <param name="info"></param>
			private void CalculateCharacteristicValues(SignalInformation info)
			{
				// Add the DC component to each characteristic
				info.Maximum += info.DC;
				info.Minimum += info.DC;
				// For RMS add RMS value of the component (simple square for DC)
				info.RMS = Math.Pow(info.DC, 2);

				// For each AC component
				foreach (var voltage in info.ComposingPhasors)
				{
					// Add it to each characteristic
					info.Maximum += voltage.Value.Magnitude;
					info.Minimum -= voltage.Value.Magnitude;
					// For RMS add RMS value of the component (square divided by square root of 2 for sine waves)
					info.RMS += Math.Pow(voltage.Value.Magnitude / Math.Sqrt(2), 2);
				}

				// Finally take a square root of RMS
				info.RMS = Math.Sqrt(info.RMS);
			}

			/// <summary>
			/// Checks if <see cref="ISignalInformation.Maximum"/> is a positive value, if not inverts the direction of voltage
			/// drops and sets the <see cref="ISignalInformation.InvertedDirection"/> flag to true
			/// </summary>
			/// <param name="info"></param>
			private void CheckIfMaximumIsPositive(SignalInformation info)
			{
				if (info.DC < 0)
				{
					NegateSignal(info);
				}
			}

			/// <summary>
			/// Negates the voltage drop (Negates all composing waveforms, rearrenges <see cref="SignalInformation.Maximum"/> and
			/// <see cref="SignalInformation.Minimum"/>, flips <see cref="SignalInformation.InvertedDirection"/> flag.
			/// </summary>
			/// <param name="info"></param>
			private void NegateSignal(SignalInformation info)
			{
				// Negate all composing waveforms
				info.ComposingPhasors = info.ComposingPhasors.Select((waveform) => new KeyValuePair<double, Complex>(
					waveform.Key, -waveform.Value));

				info.DC = -info.DC;

				// The new maximum is negated minimum and the new minimum is negated maximum (eg. -2 and -4 to 4 and 2)
				var tempMax = info.Maximum;
				info.Maximum = -info.Minimum;
				info.Minimum = -tempMax;

				// Finally set the flag
				info.InvertedDirection = !info.InvertedDirection;
			}

			/// <summary>
			/// Returns an uninverted signal. If the <paramref name="info"/> is already not inverted then simply returns it. Otherwise
			/// creates a non-inverted copy and returns it.
			/// </summary>
			/// <param name="info"></param>
			/// <returns></returns>
			private ISignalInformation GetUninvertedSignal(ISignalInformation info) =>
				!info.InvertedDirection ? info : new SignalInformation()
			{
				DC = -info.DC,
				ComposingPhasors = info.ComposingPhasors.Select((phasor) =>
					new KeyValuePair<double, Complex>(phasor.Key, -phasor.Value)),
				RMS = info.RMS,
				Maximum = -info.Minimum,
				Minimum = -info.Maximum,
			};

			/// <summary>
			/// Constructs a new VoltageDropInformation based on two nodes (with <paramref name="nodeA"/> being the reference node)
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			private SignalInformation Construct(INode nodeA, INode nodeB)
			{
				// Create a new instance
				var info = new SignalInformation();

				// Calculate DC drop
				info.DC = nodeB.DCPotential.Value - nodeA.DCPotential.Value;

				// Determine AC drops
				info.ComposingPhasors = GetACWaveforms(nodeA.ACPotentials, nodeB.ACPotentials);
				
				CalculateCharacteristicValues(info);

				CheckIfMaximumIsPositive(info);

				return info;
			}

			#endregion

			#region Current Related

			/// <summary>
			/// Returns a DC current flowing through a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private double GetPassiveTwoTerminalDCCurrent(ISignalInformation voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.DC * twoTerminal.GetConductance();

			/// <summary>
			/// Returns AC current phasors for a two terminal
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="twoTerminal"></param>
			/// <returns></returns>
			private IEnumerable<KeyValuePair<double, Complex>> GetPassiveTwoTerminalACCurrentPhasors(
				ISignalInformation voltageDrop, ITwoTerminal twoTerminal) =>
				voltageDrop.ComposingPhasors.Select((phasor) =>
				new KeyValuePair<double, Complex>(phasor.Key, phasor.Value * twoTerminal.GetAdmittance(phasor.Key)));

			/// <summary>
			/// Returns current information about a standard two terminal element. Designed for: <see cref="IResistor"/>,
			/// <see cref="ICapacitor"/>
			/// </summary>
			/// <returns></returns>
			private ISignalInformation GetStandardPassiveTwoTerminalCurrent(ISignalInformation voltageDrop, ITwoTerminal element)
			{
				// If there was a cache entry already return it
				if(_CurrentCache.TryGetValue(new Tuple<IBaseComponent, ISignalInformation>(element, voltageDrop), out var current))
				{
					return current;
				}

				// Otherwise create a new current information
				var result = new SignalInformation()
				{
					InvertedDirection = voltageDrop.InvertedDirection,
					DC = GetPassiveTwoTerminalDCCurrent(voltageDrop, element),
					ComposingPhasors = GetPassiveTwoTerminalACCurrentPhasors(voltageDrop, element),
				};

				CalculateCharacteristicValues(result);

				// Cache it
				CacheCurrent(element, voltageDrop, result);

				return result;
			}

			#endregion

			#endregion

			#region Public methods

			#region Data loading

			/// <summary>
			/// Loads new nodes based on which results are computed
			/// </summary>
			/// <param name="nodes"></param>
			public void LoadNewData(IEnumerable<INode> nodes, IEnumerable<KeyValuePair<int, ISignal>> vsCurrents)
			{
				// Clear the old, already computed entries
				ClearCaches();

				// Create a new list with nodes
				_Nodes = new List<INode>(nodes);

				// Creates a dictionary of active component currents
				_ActiveComponentsCurrentCache = new Dictionary<int, SignalInformation>(vsCurrents.ToDictionary(
					(current) => current.Key, (current) => new SignalInformation(current.Value)));

				// Calculate characteristic values for each current
				foreach(var current in _ActiveComponentsCurrentCache.Values)
				{
					CalculateCharacteristicValues(current);
				}
						
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
			public bool TryGetVoltageDrop(int nodeIndex, out ISignalInformation voltageDrop) =>
				TryGetVoltageDrop(SimulationManager.GroundNodeIndex, nodeIndex, out voltageDrop);

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node). If the node
			/// indexes exceed currently held nodes count null is assigned to <paramref name="voltageDrop"/> and false is returned
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public bool TryGetVoltageDrop(int nodeAIndex, int nodeBIndex, out ISignalInformation voltageDrop)
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
					voltageDrop = new SignalInformation();
					return true;
				}

				// If that particular voltage drop was determined already return it
				if(_VoltageDropCache.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
				{
					voltageDrop = _VoltageDropCache[new Tuple<int, int>(nodeAIndex, nodeBIndex)];
					return true;
				}

				// Otherwise construct it
				var info = Construct(_Nodes[nodeAIndex], _Nodes[nodeBIndex]);

				// Cache it
				CacheVoltageDrop(info, nodeAIndex, nodeBIndex);

				voltageDrop = info;

				// And return success
				return true;
			}

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground or returns a drop equal to zero if unsuccessful
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <returns></returns>
			public ISignalInformation GetVoltageDropOrZero(int nodeIndex) => GetVoltageDropOrZero(GroundNodeIndex, nodeIndex);				

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node) or returns a drop
			/// equal to zero if unsuccessful
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public ISignalInformation GetVoltageDropOrZero(int nodeAIndex, int nodeBIndex)
			{
				if(TryGetVoltageDrop(nodeAIndex, nodeBIndex, out var info))
				{
					return info;
				}
				else
				{
					return new SignalInformation();
				}
			}

			#endregion

			#region Current related

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="resistor"></param>
			/// <returns></returns>
			public ISignalInformation GetCurrent(ISignalInformation voltageDrop, IResistor resistor) =>
				GetStandardPassiveTwoTerminalCurrent(voltageDrop, resistor);

			/// <summary>
			/// Gets information about current flowing through an <see cref="IResistor"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="capacitor"></param>
			/// <returns></returns>
			public ISignalInformation GetCurrent(ISignalInformation voltageDrop, ICapacitor capacitor) =>
				GetStandardPassiveTwoTerminalCurrent(voltageDrop, capacitor);

			/// <summary>
			/// Returns current produced by some <see cref="IActiveComponent"/>
			/// </summary>
			/// <param name="activeComponentIndex">Index of the <see cref="IActiveComponent"/> whose current to query</param>
			/// <param name="reverseDirection">True if the direction of current should be reversed with respect to the one given
			/// by convention for the specific element</param>
			/// <returns></returns>
			public ISignalInformation GetCurrentOrZero(int activeComponentIndex, bool reverseDirection)
			{
				// If the current can be found
				if(_ActiveComponentsCurrentCache.TryGetValue(activeComponentIndex, out var signal))
				{
					// If reversion was requested
					if(reverseDirection)
					{
						// Make a copy
						signal = new SignalInformation(signal);
						// And negate it
						NegateSignal(signal);
					}

					return signal;
				}

				return new SignalInformation();
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
				if(_PowerCache.TryGetValue(new Tuple<IBaseComponent, ISignalInformation>(resistor, voltageDrop), out var power))
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
				CachePower(resistor, voltageDrop, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="ICurrentSource"/>
			/// </summary>
			/// <param name="voltageDrop"></param>
			/// <param name="currentSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(ISignalInformation voltageDrop, ICurrentSource currentSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(new Tuple<IBaseComponent, ISignalInformation>(currentSource, voltageDrop), out var power))
				{
					return power;
				}

				// If not prepare to create a new power info
				// Get non-inverted voltage drop
				var nvd = GetUninvertedSignal(voltageDrop);

				// Average is negative voltage drop times produced current (to abide passive sign convention)
				var result = new PowerInformation()
				{
					Average = -nvd.DC * currentSource.ProducedCurrent,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)
				// It's the minimum voltage drop minus twice DC voltage drop times current. (Minimum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Minimum = (nvd.Minimum - 2 * nvd.DC) * currentSource.ProducedCurrent;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				// It's the maximum voltage drop minus twice DC voltage drop times current. (Maximum already has +VDC in it so in order
				// to have -VDC in total there's -2VDC. We need to subtract DC due to passive sign convention)
				result.Maximum = (nvd.Maximum - 2 * nvd.DC) * currentSource.ProducedCurrent;

				// Cache the calculated value
				CachePower(currentSource, voltageDrop, result);

				// And return it
				return result;
			}

			/// <summary>
			/// Gets information about power on an <see cref="IVoltageSource"/>
			/// </summary>
			/// <param name="current"></param>
			/// <param name="voltageSource"></param>
			/// <returns></returns>
			public IPowerInformation GetPower(ISignalInformation current, IVoltageSource voltageSource)
			{
				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(new Tuple<IBaseComponent, ISignalInformation>(voltageSource, current), out var power))
				{
					return power;
				}

				// If not prepare to create a new power info
				// Get non-inverted current
				var nc = GetUninvertedSignal(current);

				// Average is voltage drop times produced current (current is assumed to flow right to left in standard convention,
				// current produced flows left to right so produced power is negative)
				var result = new PowerInformation()
				{
					Average = nc.DC * voltageSource.ProducedDCVoltage,
				};

				// Minimum power (the maximum supplied or the least dissipated, depending on actual values)				
				result.Minimum = nc.Minimum * voltageSource.ProducedDCVoltage;

				// Maximum power (the maximum dissipated or the least supplied, depending on actual values)
				result.Maximum = nc.Maximum * voltageSource.ProducedDCVoltage;

				// Cache the calculated value
				CachePower(voltageSource, current, result);

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
			public IPowerInformation GetPower(ISignalInformation current, IACVoltageSource voltageSource)
			{
				// TODO: When time-based simulation is implemented try to calcuate the average iteratively if there is more than one
				// current phasor - for example as an average of power calculated for n points in one full cycle

				// Check if there already is a cached entry
				if (_PowerCache.TryGetValue(new Tuple<IBaseComponent, ISignalInformation>(voltageSource, current), out var power))
				{
					return power;
				}

				// If not prepare to create a new power info
				// Get non-inverted current
				var nc = GetUninvertedSignal(current);

				// Create a new info
				var result = new PowerInformation();

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
						var singlePhasors = nc.ComposingPhasors.First();

						// Calculate the average as Vrms*Irms*cos(phiV - phiI)
						// TODO: When IAsyncVoltageSource has phase shift, include it in the formula
						result.Average = nc.RMS * Math.Sqrt(Math.Pow(voltageSource.ProducedDCVoltage, 2) +
							Math.Pow(voltageSource.PeakProducedVoltage, 2) / 2) * Math.Cos(singlePhasors.Value.Phase);
					}
				}

				// TODO: Try to think of a way to calculate max/min instantenous power of the source

				// Cache the calculated value
				CachePower(voltageSource, current, result);

				// And return it
				return result;
			}

			#endregion

			#endregion
		}
	}
}