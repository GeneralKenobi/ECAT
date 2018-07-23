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
		double ProducedCurrent { get; }

		#endregion
	}
}