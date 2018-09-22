using System;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for Attributes related to display info, carries information about section's final position.
	/// </summary>
	public abstract class DisplayInfo : Attribute
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="sectionIndex">Nonnegative integer</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public DisplayInfo(int sectionIndex)
		{
			SectionIndex = sectionIndex >= 0 ?
				sectionIndex : throw new ArgumentOutOfRangeException(nameof(sectionIndex) + " has to be nonnegative");
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Index for the section. Index 0 is the first one. Negative indexes cause an exception. Sections with equal indexes have
		/// their final section position resolved randomly.
		/// </summary>
		public int SectionIndex { get; }

		#endregion
	}
}