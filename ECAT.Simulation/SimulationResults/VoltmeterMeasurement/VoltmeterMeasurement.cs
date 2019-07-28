using ECAT.Core;

namespace ECAT.Simulation
{
	/// <summary>
	/// Standard implementation of <see cref="IVoltmeterMeasurement"/>
	/// </summary>
	[RegisterAsType(typeof(IVoltmeterMeasurement))]
	public class VoltmeterMeasurement : IVoltmeterMeasurement
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public VoltmeterMeasurement(string voltmeterID, int nodeA, int nodeB)
		{
			VoltmeterID = voltmeterID;
			NodeA = nodeA;
			NodeB = nodeB;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// ID of the voltmeter (used to the user to differentiate between voltmeters)
		/// </summary>
		public string VoltmeterID { get; }

		/// <summary>
		/// Reference node of the voltmeter
		/// </summary>
		public int NodeA { get; }

		/// <summary>
		/// Non-reference node of the voltmeter
		/// </summary>
		public int NodeB { get; }

		#endregion
	}
}
