using System.Numerics;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for all components that have only one terminal 
	/// </summary>
	public interface IOneTerminal
    {
		#region Properties

		/// <summary>
		/// One of the terminals in this two-terminal
		/// </summary>
		ITerminal Terminal { get; }
		
		/// <summary>
		/// The input admittance of the component
		/// </summary>
		Complex Admittance { get; set; }

		#endregion
	}
}