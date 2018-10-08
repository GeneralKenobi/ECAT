using CSharpEnhanced.Helpers;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Interface between the admittance matrix constructor (<see cref="AdmittanceMatrixCore"/>) and users
	/// </summary>
	public class AdmittanceMatrix : AdmittanceMatrixCore
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		private AdmittanceMatrix(ISchematic schematic) : base(schematic)
		{
			
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Used to store op-amps that were determined to be saturated
		/// </summary>
		private List<int> _SaturatedOpAmps { get; set; } = new List<int>();

		/// <summary>
		/// Used to store tested combinations of opamps when searching for false-positives
		/// </summary>
		private BitArray _TestedCombinations { get; set; }
		
		private Dictionary<INode, IPhasorDomainSignalMutable> _Results { get; set; }

		#endregion

		#region Private methods

		/// <summary>
		/// Solve the system for the current configuration
		/// </summary>
		private Complex[] Solve(bool adjustOpAmps)
		{
			Complex[] result = null;

			// Keep calculating results and adjusting op-amps into saturation until all op-amps are within their supply voltages
			do
			{
				try
				{
					result = LinearEquations.SimplifiedGaussJordanElimination(ComputeCoefficientMatrix(), ComputeFreeTermsMatrix(), true);
					if(!adjustOpAmps)
					{
						return result;
					}
				}
				catch (Exception)
				{
					return ArrayHelpers.CreateAndInitialize<Complex>(0, _Size);
				}

			} while (CheckOpAmpOperation(result));

			_TestedCombinations = new BitArray(_SaturatedOpAmps.Count, true);
			
			do
			{
				CheckOpAmpOperationFalsePositives(result);

				try
				{
					result = LinearEquations.SimplifiedGaussJordanElimination(ComputeCoefficientMatrix(), ComputeFreeTermsMatrix(), true);
				}
				catch (Exception)
				{
					return ArrayHelpers.CreateAndInitialize<Complex>(0, _Size);
				}

			} while (CheckOpAmpOperation(result));

			return result;
		}

		/// <summary>
		/// Checks if <see cref="IOpAmp"/>s operate in the proper region (if they didn't exceed their supply voltages.) If they don't,
		/// adjusts the <see cref="_B"/>, <see cref="_C"/> and <see cref="_E"/> matrices so that, instead of being variable voltage
		/// sources, they are independent voltage sources capped at their supply voltage value. Returns true if an <see cref="IOpAmp"/>
		/// was adjusted and calculations need to be redone, false otherwise
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool CheckOpAmpOperation(Complex[] result)
		{
			for (int i = 0; i < OpAmpsCount; ++i)
			{
				// The considered op-amp
				var opAmpInfo = _OpAmpOutputs[i];

				// If the output is not grounded TODO: remove the check then the rule that voltage source outputs are not allowed to be
				// grounded is enforced
				if (opAmpInfo.Item1 != -1 && (result[opAmpInfo.Item1].Real < opAmpInfo.Item2 ||
					result[opAmpInfo.Item1].Real > opAmpInfo.Item3))
				{
					// Op-amp needs adjusting, it's output will now be modeled as an independent voltage source now
					ConfigureForSaturation(i, result[opAmpInfo.Item1].Real > opAmpInfo.Item2);
					_SaturatedOpAmps.Add(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if <see cref="IOpAmp"/>s operate in the proper region (if they didn't exceed their supply voltages.) If they don't,
		/// adjusts the <see cref="_B"/>, <see cref="_C"/> and <see cref="_E"/> matrices so that, instead of being variable voltage
		/// sources, they are independent voltage sources capped at their supply voltage value. Returns true if an <see cref="IOpAmp"/>
		/// was adjusted and calculations need to be redone, false otherwise
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private void CheckOpAmpOperationFalsePositives(Complex[] result)
		{
			// TODO: Start checking by disabling all op-amps and selectively enabling them
			for (int i = 0; i < _TestedCombinations.Count; i++)
			{
				bool previous = _TestedCombinations[i];
				_TestedCombinations[i] = !previous;
				if (previous)
				{
					// Found a clear bit - now that we've set it, we're done
					ConfigureForActiveOperation(_SaturatedOpAmps[i]);
					return;
				}

			}
		}

		#endregion

		#region Public methods

		private void AssignDCResultsA(IEnumerable<KeyValuePair<INode, IPhasorDomainSignalMutable>> nodePotentials,
			IEnumerable<KeyValuePair<int, IPhasorDomainSignalMutable>> activeComponentsCurrents, Complex[] results)
		{
			_Results.ForEach((x, i) => x.Value.SetDC(results[i].Real));
		}

		private void AssignACResultsA(double frequency, Complex[] results)
		{
			_Results.ForEach((x, i) => x.Value.AddPhasor(frequency, results[i]));
		}

		/// <summary>
		/// Performs a bias simulation of the schematic - calculates symbolical, time-independent voltages and currents produced by
		/// voltage sources.
		/// </summary>
		/// <param name="simulationType"></param>
		public void Bias(SimulationType simulationType, out IEnumerable<KeyValuePair<INode, IPhasorDomainSignal>> nodePotentials,
			out IEnumerable<KeyValuePair<int, IPhasorDomainSignal>> activeComponentsCurrents)
		{
			var potentials = new Dictionary<INode, IPhasorDomainSignalMutable>(GetNodes().
				ToDictionary((x) => x, (x) => IoC.Resolve<IPhasorDomainSignalMutable>()));

			var activeCurrents = new Dictionary<int, IPhasorDomainSignalMutable>(_ActiveComponentsCount.ToSequence().
				ToDictionary((x) => x, (x) => IoC.Resolve<IPhasorDomainSignalMutable>()));

			// If DC bias is specified
			if (simulationType.HasFlag(SimulationType.DC))
			{
				// Configure for DC and assign result to combinedResult
				ConfigureForFrequency(0);

				var result = Solve(!simulationType.HasFlag(SimulationType.AC));

				for(int i=0; i< result.Length; ++i)
				{
					if(i < potentials.Count)
					{
						potentials.ElementAt(i).Value.SetDC(result[i].Real);
					}
					else
					{
						activeCurrents.ElementAt(i - potentials.Count).Value.SetDC(result[i].Real);
					}
				}
			}

			// If AC is specified
			if (simulationType.HasFlag(SimulationType.AC))
			{
				// For each frequency in the circuit
				for (int i = 0; i < _FrequenciesInCircuit.Count; ++i)
				{
					// Configure the matrix for that frequency
					ConfigureForFrequency(_FrequenciesInCircuit[i]);

					// Get a subresult
					AssignACResultsA(_FrequenciesInCircuit[i], Solve(false));
					var result = Solve(false);

					for (int j = 0; j < result.Length; ++j)
					{
						if (j < potentials.Count)
						{
							potentials.ElementAt(j).Value.AddPhasor(_FrequenciesInCircuit[i], result[j]);
						}
						else
						{
							activeCurrents.ElementAt(j - potentials.Count).Value.AddPhasor(_FrequenciesInCircuit[i], result[j]);
						}
					}
				}
			}

			nodePotentials = potentials.ToDictionary((x) => x.Key, (x) => (IPhasorDomainSignal)x.Value);
			activeComponentsCurrents = activeCurrents.ToDictionary((x) => x.Key, (x) => (IPhasorDomainSignal)x.Value);
		}

		#endregion
		
		#region Public static methods

		/// <summary>
		/// Constructs and returns an admittance matrix
		/// </summary>
		/// <param name="schematic"></param>
		/// <returns></returns>
		public static bool Construct(ISchematic schematic, out AdmittanceMatrix matrix, out string errorMessage)
		{
			matrix = new AdmittanceMatrix(schematic);

			try
			{
				matrix.Build();
			} catch(Exception e)
			{
				matrix = null;
				errorMessage = e.Message;
				return false;
			}

			errorMessage = string.Empty;
			return true;
		}

		#endregion
	}
}