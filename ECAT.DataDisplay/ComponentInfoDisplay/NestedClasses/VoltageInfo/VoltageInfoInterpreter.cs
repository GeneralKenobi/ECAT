namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Interprets signals related to voltage
		/// </summary>
		private class VoltageInfoInterpreter : GenericSignalInformationInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public VoltageInfoInterpreter() : base("Voltage", "V") { }

			#endregion
		}
	}
}