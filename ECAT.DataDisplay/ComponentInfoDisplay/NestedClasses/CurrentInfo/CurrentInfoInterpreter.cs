namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Interprets signals related to current
		/// </summary>
		private class CurrentInfoInterpreter : GenericSignalInformationInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public CurrentInfoInterpreter() : base("Current", "A") { }

			#endregion
		}
	}
}