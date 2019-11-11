using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to register an <see cref="IBaseComponent"/>'s current flow information. It is aimed for all components that only
	/// have one notable current flow (all <see cref="ITwoTerminal"/>, <see cref="IOpAmp"/> (only output current)).
	/// Using it on classes that don't implement <see cref="IBaseComponent"/> will not do anything.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class DisplayCurrentInfo : DisplayInfo
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="header">Header displayed above the info section</param>
		/// <param name="sectionIndex">Final position of the section, nonnegative, default value is <see cref="int.MaxValue"/></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayCurrentInfo(string header = "Current", int sectionIndex = int.MaxValue - 1) : base(sectionIndex, header) { }

		#endregion
	}
}