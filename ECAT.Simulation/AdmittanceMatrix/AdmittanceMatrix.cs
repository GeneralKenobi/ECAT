using CSharpEnhanced.Maths;
using ECAT.Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
		private AdmittanceMatrix(ISchematic schematic) : base(schematic) { }

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

		#endregion

		#region Private methods

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

		/// <summary>
		/// Solve the system for the current configuration
		/// </summary>
		public void Solve()
		{
			Complex[] result = null;

			// Keep calculating results and adjusting op-amps into saturation until all op-amps are within their supply voltages
			do
			{
				try
				{
					result = LinearEquations.SimplifiedGaussJordanElimination(ComputeCoefficientMatrix(), ComputeFreeTermsMatrix());
				}
				catch (Exception)
				{
					return;
				}

			} while (CheckOpAmpOperation(result));

			_TestedCombinations = new BitArray(_SaturatedOpAmps.Count, true);

			
			do
			{
				CheckOpAmpOperationFalsePositives(result);

				try
				{
					result = LinearEquations.SimplifiedGaussJordanElimination(ComputeCoefficientMatrix(), ComputeFreeTermsMatrix());
				}
				catch (Exception)
				{
					return;
				}

			} while (CheckOpAmpOperation(result));

			AssignResults(result);
		}

		/// <summary>
		/// Activates all DC voltage sources
		/// </summary>
		public void ActivateDCVoltageSources()
		{
			for (int i = 0; i < DCVoltageSourcesCount; ++i)
			{
				ActivateDCVoltageSource(i);
			}
		}

		/// <summary>
		/// Dezactivates all DC voltage sources
		/// </summary>
		public void DezactivateDCVoltageSources()
		{
			for (int i = 0; i < DCVoltageSourcesCount; ++i)
			{
				DezactivateDCVoltageSource(i);
			}
		}

		/// <summary>
		/// Activates all DC voltage sources
		/// </summary>
		public void ActivateCurrentSources()
		{
			for (int i = 0; i < CurrentSourcesCount; ++i)
			{
				ActivateCurrentSource(i);
			}
		}

		/// <summary>
		/// Dezactivates all DC voltage sources
		/// </summary>
		public void DezactivateCurrentSources()
		{
			for (int i = 0; i < CurrentSourcesCount; ++i)
			{
				DezactivateCurrentSource(i);
			}
		}

		/// <summary>
		/// Activates all AC voltage sources
		/// </summary>
		public void ActivateACVoltageSources()
		{
			for (int i = 0; i < ACVoltageSourcesCount; ++i)
			{
				ActivateACVoltageSource(i);
			}
		}

		/// <summary>
		/// Dezactivates all AC voltage sources
		/// </summary>
		public void DezactivateACVoltageSources()
		{
			for (int i = 0; i < ACVoltageSourcesCount; ++i)
			{
				DezactivateACVoltageSource(i);
			}
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