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
		private class SimulationResultManager : ISimulationResultManager
		{
			#region Private properties

			/// <summary>
			/// List with all nodes upon which specific results are calculated
			/// </summary>
			private List<INode> _Nodes { get; set; } = new List<INode>();

			/// <summary>
			/// Dictionary holding already computed voltage drops for the last performed simulation. Ints in key tuple are indexes of
			/// nodes (Item1 for the first node (reference node) and Item2 for the second node (target node))
			/// </summary>
			private Dictionary<Tuple<int, int>, VoltageDropInformation> _AlreadyComputed { get; } =
				new Dictionary<Tuple<int, int>, VoltageDropInformation>(new CustomEqualityComparer<Tuple<int, int>>(
					// Compare the elements of the Tuples, now tuples themselves
					(x, y) => x.Item1 == y.Item1 && x.Item2 == y.Item2));

			#endregion

			#region Private methods

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
					yield return new KeyValuePair<double, Complex>(key, -nodeBACPotentials[key]);
				}
			}

			/// <summary>
			/// Sets the types of waveforms that compose the voltage drop
			/// </summary>
			/// <param name="info"></param>
			private void SetFlags(VoltageDropInformation info)
			{
				// If DC drop is not 0, set its flag
				if (info.DC != 0)
				{
					info.Type |= VoltageDropType.DC;
				}

				// Get the number of ac waveforms
				var acWaveformsCount = info.ComposingACWaveforms.Count();

				// If it's greater than 0 set some AC flag
				if (acWaveformsCount > 0)
				{
					// And greater than 1 set multiple flag
					if (acWaveformsCount > 1)
					{
						info.Type |= VoltageDropType.MultipleAC;
					}
					// If it's not (which means there's only 1) set the single flag
					else
					{
						info.Type |= VoltageDropType.SingleAC;
					}
				}
			}

			/// <summary>
			/// Calculates and assigns characteristic voltages - maximum, minimum, RMS
			/// </summary>
			/// <param name="info"></param>
			private void CalculateCharacteristicVoltages(VoltageDropInformation info)
			{
				// Add the DC component to each characteristic
				info.Maximum += info.DC;
				info.Minimum += info.DC;
				// For RMS add RMS value of the component (simple square for DC)
				info.RMS = Math.Pow(info.DC, 2);

				// For each AC component
				foreach (var voltage in info.ComposingACWaveforms)
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
			/// Checks if <see cref="IVoltageDropInformation.Maximum"/> is a positive value, if not inverts the direction of voltage
			/// drops and sets the <see cref="IVoltageDropInformation.InvertedDirection"/> flag to true
			/// </summary>
			/// <param name="info"></param>
			private void CheckIfMaximumIsPositive(VoltageDropInformation info)
			{
				if (info.Maximum < 0)
				{
					NegateVoltageDrop(info);
				}
			}

			/// <summary>
			/// Negates the voltage drop (Negates all composing waveforms, rearrenges <see cref="VoltageDropInformation.Maximum"/> and
			/// <see cref="VoltageDropInformation.Minimum"/>, flips <see cref="VoltageDropInformation.InvertedDirection"/> flag.
			/// </summary>
			/// <param name="info"></param>
			private void NegateVoltageDrop(VoltageDropInformation info)
			{
				// Negate all composing waveforms
				info.ComposingACWaveforms = info.ComposingACWaveforms.Select((waveform) => new KeyValuePair<double, Complex>(
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
			/// Constructs a new VoltageDropInformation based on two nodes (with <paramref name="nodeA"/> being the reference node)
			/// </summary>
			/// <param name="nodeA"></param>
			/// <param name="nodeB"></param>
			/// <returns></returns>
			private VoltageDropInformation Construct(INode nodeA, INode nodeB)
			{
				// Create a new instance
				var info = new VoltageDropInformation();

				// Calculate DC drop
				info.DC = nodeB.DCPotential.Value - nodeA.DCPotential.Value;

				// Determine AC drops
				info.ComposingACWaveforms = GetACWaveforms(nodeA.ACPotentials, nodeB.ACPotentials);
				
				SetFlags(info);

				CalculateCharacteristicVoltages(info);

				CheckIfMaximumIsPositive(info);

				return info;
			}

			/// <summary>
			/// Caches the <paramref name="info"/> as well as its copy with inverted indexes into <see cref="_AlreadyComputed"/>
			/// </summary>
			/// <param name="info"></param>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			private void Cache(VoltageDropInformation info, int nodeAIndex, int nodeBIndex)
			{
				// Get a copy for opposite node mapping
				var copy = info.Copy();

				// Negate it
				NegateVoltageDrop(copy);

				// Cache the original
				_AlreadyComputed.Add(new Tuple<int, int>(nodeAIndex, nodeBIndex), info);

				// And cache the copy
				_AlreadyComputed.Add(new Tuple<int, int>(nodeBIndex, nodeAIndex), copy);
			}

			#endregion

			#region Public methods

			/// <summary>
			/// Gets a voltage drop of a node with respect to ground
			/// </summary>
			/// <param name="nodeIndex"></param>
			/// <returns></returns>
			public bool TryGetVoltageDrop(int nodeIndex, out IVoltageDropInformation voltageDrop) =>
				TryGetVoltageDrop(_GroundNodeIndex, nodeIndex, out voltageDrop);

			/// <summary>
			/// Gets information on voltage drop between two nodes (with node A being treated as the reference node). If the node
			/// indexes exceed currently held nodes count null is assigned to <paramref name="voltageDrop"/> and false is returned
			/// </summary>
			/// <param name="nodeAIndex"></param>
			/// <param name="nodeBIndex"></param>
			/// <returns></returns>
			public bool TryGetVoltageDrop(int nodeAIndex, int nodeBIndex, out IVoltageDropInformation voltageDrop)
			{
				if(nodeAIndex >= _Nodes.Count || nodeBIndex >= _Nodes.Count)
				{
					voltageDrop = null;
					return false;
				}

				// If that particular voltage drop was determined already return it
				if(_AlreadyComputed.ContainsKey(new Tuple<int, int>(nodeAIndex, nodeBIndex)))
				{
					voltageDrop = _AlreadyComputed[new Tuple<int, int>(nodeAIndex, nodeBIndex)];
					return true;
				}

				// Otherwise construct it
				var info = Construct(_Nodes[nodeAIndex], _Nodes[nodeBIndex]);

				// Cache it
				Cache(info, nodeAIndex, nodeBIndex);

				voltageDrop = info;

				// And return success
				return true;
			}

			/// <summary>
			/// Loads new nodes based on which results are computed
			/// </summary>
			/// <param name="nodes"></param>
			public void LoadNewNodes(IEnumerable<INode> nodes)
			{
				// Clear the old, already computed entries
				_AlreadyComputed.Clear();

				// Create a new list with nodes
				_Nodes = new List<INode>(nodes);
			}

			#endregion

			#region Private static properties

			/// <summary>
			/// Index of the ground node, by convention -1
			/// </summary>
			private static int _GroundNodeIndex { get; } = -1;

			#endregion
		}
	}
}