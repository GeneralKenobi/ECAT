using CSharpEnhanced.Maths;
using System;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Admittance matrix that describes linearly dependences between node potentials, currents and admittances in a circuit. It can be solved
	/// as a system of linear equations in order to obtain complete information on the circuit.
	/// </summary>
	public class AdmittanceMatrix
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="bigDimension">Dimension of A sub-matrix</param>
		/// <param name="smallDimension">Dimension of D sub-matrix</param>
		public AdmittanceMatrix(int bigDimension, int smallDimension)
		{
			_BigDimension = bigDimension > 0 ? bigDimension : throw new ArgumentException(nameof(bigDimension) + " can't be smaller than 1");
			_SmallDimension = smallDimension > 0 ? smallDimension : throw new ArgumentException(nameof(smallDimension) + " can't be smaller than 1");
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="_A"/>
		/// </summary>
		private Complex[,] mA;

		/// <summary>
		/// Backing store for <see cref="_B"/>
		/// </summary>
		private Complex[,] mB;

		/// <summary>
		/// Backing store for <see cref="_C"/>
		/// </summary>
		private Complex[,] mC;

		/// <summary>
		/// Backing store for <see cref="_D"/>
		/// </summary>
		private Complex[,] mD;

		/// <summary>
		/// Backing store for <see cref="_I"/>
		/// </summary>
		private Complex[] mI;

		/// <summary>
		/// Backing store for <see cref="_E"/>
		/// </summary>
		private Complex[] mE;

		#endregion

		#region Public properties

		#region Dimension

		/// <summary>
		/// Size of A part of the admittance matrix (dependent on nodes)
		/// </summary>
		public int _BigDimension { get; }

		/// <summary>
		/// Size of D part of admittance matrix (depends on the number of independent voltage sources, with op-amp outputs included)
		/// </summary>
		public int _SmallDimension { get; }

		/// <summary>
		/// Size of the whole admittance matrix
		/// </summary>
		public int _Size => _BigDimension + _SmallDimension;

		#endregion

		#region Submatrices

		/// <summary>
		/// Part of admittance matrix located in the top left corner, built based on nodes and admittances connected to them.
		/// Should be <see cref="_BigDimension"/> by <see cref="_BigDimension"/>.
		/// </summary>
		public Complex[,] _A
		{
			get => mA;
			set
			{
				if(value.GetLength(0) == _BigDimension && value.GetLength(1) == _BigDimension)
				{
					mA = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		/// <summary>
		/// Part of admittance matrix located in the top right corner - based on independent voltage sources (including op-amp outputs).
		/// Should be <see cref="_BigDimension"/> by <see cref="_SmallDimension"/>.
		/// </summary>
		public Complex[,] _B
		{
			get => mB;
			set
			{
				if (value.GetLength(0) == _BigDimension && value.GetLength(1) == _SmallDimension)
				{
					mB = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		/// <summary>
		/// Part of admittance matrix located in the bottom left corner - based on independent voltage sources (excluding op-amp outputs)
		/// and inputs of op-amps.
		/// Should be <see cref="_SmallDimension"/> by <see cref="_BigDimension"/>.
		/// </summary>
		public Complex[,] _C
		{
			get => mC;
			set
			{
				if (value.GetLength(0) == _SmallDimension && value.GetLength(1) == _BigDimension)
				{
					mC = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		/// <summary>
		/// Part of admittance matrix located in the bottom right corner - based on dependent sources
		/// Should be <see cref="_SmallDimension"/> by <see cref="_SmallDimension"/>.
		/// </summary>
		public Complex[,] _D
		{
			get => mD;
			set
			{
				if (value.GetLength(0) == _SmallDimension && value.GetLength(1) == _SmallDimension)
				{
					mD = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		/// <summary>
		/// Top part of the vector of free terms, based on current sources.
		/// Should be <see cref="_BigDimension"/> long.
		/// </summary>
		public Complex[] _I
		{
			get => mI;
			set
			{
				if (value.GetLength(0) == _BigDimension)
				{
					mI = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		/// <summary>
		/// Bottom part of the vector of free terms, based on voltage sources (including op-amp outputs)
		/// Should be <see cref="_SmallDimension"/> long.
		/// </summary>
		public Complex[] _E
		{
			get => mE;
			set
			{
				if (value.GetLength(0) == _SmallDimension)
				{
					mE = value;
				}
				else
				{
					throw new ArgumentException("Incompatible dimensions");
				}
			}
		}

		#endregion

		#endregion

		#region Protected methods

		/// <summary>
		/// Combines matrices <see cref="_A"/>, <see cref="_B"/>, <see cref="_C"/>, <see cref="_D"/> to create admittance matrix
		/// </summary>
		protected Complex[,] ComputeCoefficientMatrix()
		{
			var result = new Complex[_Size, _Size];

			// _A
			for (int rowIndex = 0; rowIndex < _BigDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _BigDimension; ++columnIndex)
				{
					result[rowIndex, columnIndex] = _A[rowIndex, columnIndex];
				}
			}

			// _B
			for (int rowIndex = 0; rowIndex < _BigDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _SmallDimension; ++columnIndex)
				{
					result[rowIndex, columnIndex + _BigDimension] = _B[rowIndex, columnIndex];
				}
			}

			// _C
			for (int rowIndex = 0; rowIndex < _SmallDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _BigDimension; ++columnIndex)
				{
					result[rowIndex + _BigDimension, columnIndex] = _C[rowIndex, columnIndex];
				}
			}

			// _D
			for (int rowIndex = 0; rowIndex < _SmallDimension; ++rowIndex)
			{
				for (int columnIndex = 0; columnIndex < _SmallDimension; ++columnIndex)
				{
					result[rowIndex + _BigDimension, columnIndex + _BigDimension] = _D[rowIndex, columnIndex];
				}
			}

			return result;
		}

		/// <summary>
		/// Combines matrices <see cref="_I"/> and <see cref="_E"/> to create a vector of free terms for the admittance matrix
		/// </summary>
		/// <returns></returns>
		protected Complex[] ComputeFreeTermsMatrix()
		{
			var result = new Complex[_BigDimension + _SmallDimension];

			// _I
			for (int i = 0; i < _BigDimension; ++i)
			{
				result[i] = _I[i];
			}

			// _E
			for (int i = 0; i < _SmallDimension; ++i)
			{
				result[i + _BigDimension] = _E[i];
			}

			return result;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns the solution of this admittance matrix
		/// </summary>
		/// <returns></returns>
		public void Solve(out Complex[] nodePotentials, out Complex[] activeComponentsCurrents)
		{
			// TODO: Add error handling (not all matrices are solvable)

			// Calculate the solution
			var solution = LinearEquations.SimplifiedGaussJordanElimination(ComputeCoefficientMatrix(), ComputeFreeTermsMatrix(), true);

			// Initialize the arrays
			nodePotentials = new Complex[_BigDimension];
			activeComponentsCurrents = new Complex[_SmallDimension];

			// The first _BigDimension entries correspond to node potentials
			for(int i=0; i<_BigDimension; ++i)
			{
				nodePotentials[i] = solution[i];
			}

			// The remaining (last _SmallDimension) entries correspond to active components currents
			for (int i = 0; i < _SmallDimension; ++i)
			{
				activeComponentsCurrents[i] = solution[i + _BigDimension];
			}
		}

		#endregion
	}
}