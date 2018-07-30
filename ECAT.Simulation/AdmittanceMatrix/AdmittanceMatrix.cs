using CSharpEnhanced.CoreClasses;
using CSharpEnhanced.Maths;
using ECAT.Core;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Simulation
{
	/// <summary>
	/// Base class for admittance matrices
	/// </summary>
	public abstract class AdmittanceMatrix
    {
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		protected AdmittanceMatrix(IExpression[,] aMatrix, IExpression[] zMatrix, List<INode> nodes, List<IVoltageSource> sources)
		{
			_A = aMatrix;
			_Z = zMatrix;

			_NodePotentials = new List<RefWrapperPropertyChanged<double>>(nodes.Select((node) => node.Potential));
			_VoltageSourcesCurrents = new List<RefWrapperPropertyChanged<double>>(sources.Select((source) => source.ProducedCurrent));			
		}

		#endregion

		#region Private properties

		/// <summary>
		/// The A admittance matrix (matrix of coefficients), rules of construction are described a separate text file
		/// </summary>
		protected IExpression[,] _A { get; }

		/// <summary>
		/// The Z matrix (matrix of free terms), rules of construction are described in a separate text file
		/// </summary>
		protected IExpression[] _Z { get; }

		/// <summary>
		/// List of references to all node potential variables
		/// </summary>
		protected List<RefWrapperPropertyChanged<double>> _NodePotentials { get; }

		/// <summary>
		/// List of references to all currents through voltage sources
		/// </summary>
		protected List<RefWrapperPropertyChanged<double>> _VoltageSourcesCurrents { get; }

		#endregion

		#region Public methods

		/// <summary>
		/// Solves the matrix for the parameter values present at the moment of calling,
		/// updates the values of nodes and sources currents
		/// </summary>
		public abstract void Solve();

		#endregion;
	}
}