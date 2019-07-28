using System;

namespace ECAT.Core
{
	/// <summary>
	/// Interface for a declaration of a voltmeter measurement
	/// </summary>
	[NecessaryService]
	[ConstructorDeclaration(new Type[] { typeof(string), typeof(int), typeof(int) }, "VoltmeterID", "NodeA", "NodeB")]
	public interface IVoltmeterMeasurement
	{
		#region Properties

		/// <summary>
		/// ID of the voltmeter (used to the user to differentiate between voltmeters)
		/// </summary>
		string VoltmeterID { get; }

		/// <summary>
		/// Reference node of the voltmeter
		/// </summary>
		int NodeA { get; }

		/// <summary>
		/// Non-reference node of the voltmeter
		/// </summary>
		int NodeB { get; }

		#endregion
	}
}