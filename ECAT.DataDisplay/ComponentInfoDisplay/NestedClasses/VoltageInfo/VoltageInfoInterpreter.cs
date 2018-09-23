using ECAT.Core;

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
			public VoltageInfoInterpreter() : base(IoC.Resolve<IQuantityNames>().VoltageCap, IoC.Resolve<ISIUnits>().VoltageShort) { }

			#endregion
		}
	}
}