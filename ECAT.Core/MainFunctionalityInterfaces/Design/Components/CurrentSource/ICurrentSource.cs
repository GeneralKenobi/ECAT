using CSharpEnhanced.Maths;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for current sources
	/// </summary>
    public interface ICurrentSource : ITwoTerminal
    {
		#region Properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		Variable ProducedCurrentVar { get; }

		/// <summary>
		/// Accessor to the produced current of this <see cref="ICurrentSource"/>
		/// </summary>
		double ProducedCurrent { get; set; }

		#endregion
	}
}