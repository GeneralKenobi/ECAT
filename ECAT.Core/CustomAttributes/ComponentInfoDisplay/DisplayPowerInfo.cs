using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to register an <see cref="IBaseComponent"/>'s power dissipation information.
	/// Using it on classes that don't implement <see cref="IBaseComponent"/> will not do anything.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class DisplayPowerInfo : DisplayInfo
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="header">Header displayed above the info section</param>
		/// <param name="sectionIndex">Final position of the section, nonnegative, default value is <see cref="int.MaxValue"/></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayPowerInfo(string header = "Power", int sectionIndex = int.MaxValue) : base(sectionIndex, header) { }

		#endregion
	}
}