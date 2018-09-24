using ECAT.Core;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Interprets signals related to 
		/// </summary>
		private class PowerInfoInterpreter : GenericSignalInformationInterpreter
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			public PowerInfoInterpreter() : base(IoC.Resolve<IQuantityNames>().PowerCap, IoC.Resolve<ISIUnits>().PowerShort) { }

			#endregion
		}
	}
}