using ECAT.Core;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
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
			public CurrentInfoInterpreter() : base(IoC.Resolve<IQuantityNames>().CurrentCap, IoC.Resolve<ISIUnits>().CurrentShort) { }

			#endregion
		}
	}
}