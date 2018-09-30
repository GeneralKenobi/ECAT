using System;
using ECAT.Core;

namespace ECAT.DataDisplay
{
	/// <summary>
	/// Standard class responsible for providing description of signals in <see cref="ISignalInformation"/>
	/// </summary>
	internal class SignalDescription : ISignalDescription
    {
		#region Constructors

		/// <summary>
		/// Default constructor, requires parameters
		/// </summary>
		/// <param name="unit"></param>
		/// <param name="unitCap"></param>
		/// <param name="unitShort"></param>
		/// <param name="name"></param>
		/// <param name="nameCap"></param>
		public SignalDescription(string unit, string unitCap, string unitShort, string name, string nameCap)
		{
			Unit = unit ?? throw new ArgumentNullException(nameof(unit));
			UnitCap = unitCap ?? throw new ArgumentNullException(nameof(unitCap));
			UnitShort = unitShort ?? throw new ArgumentNullException(nameof(unitShort));
			Name = name ?? throw new ArgumentNullException(nameof(name));
			NameCap = nameCap ?? throw new ArgumentNullException(nameof(nameCap));
		}

		#endregion

		#region Properties

		/// <summary>
		/// Full name of the unit of this signal (eg. "ampere")
		/// </summary>
		public string Unit { get; }

		/// <summary>
		/// Full name of the unit of this signal with first letter capitilized (eg. "Ampere")
		/// </summary>
		public string UnitCap { get; }

		/// <summary>
		/// Short name of the unit of this signal (eg. "A")
		/// </summary>
		public string UnitShort { get; }

		/// <summary>
		/// Name of the signal (eg. "current")
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Name of the signal with first letter capitilized (eg. "Current")
		/// </summary>
		public string NameCap { get; }

		#endregion
	}
}