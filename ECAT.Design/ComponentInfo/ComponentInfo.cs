using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IComponentInfo"/>
	/// </summary>
	public class ComponentInfo : IComponentInfo
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ComponentInfo(IEnumerable<string> headers)
		{
			// Variable used to index the headers when creating _SectionHeaders list
			int headerIndex = 0;

			// Copy the header text and complement it with index
			_SectionHeaders = new List<ComponentInfoSectionHeader>(headers.Select((headerText) =>
				// Use the headerIndex as index (and increment it for next headerText)
				new ComponentInfoSectionHeader(headerIndex++, headerText)));

			// Fill info with initially empty enumerations (the same number as number of headers)
			_Info = new List<IEnumerable<string>>(headers.Select((header) => Enumerable.Empty<string>()));

			// If there are some headers defined, mark the selected one
			if(_SectionHeaders.Count > 0)
			{
				_SectionHeaders[CurrentSectionIndex].IsSelected = true;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private members
		
		/// <summary>
		/// Backing store for <see cref="CurrentSectionIndex"/>
		/// </summary>
		private int mCurrentSectionIndex;

		#endregion

		#region Private properties

		/// <summary>
		/// Backing store for <see cref="InfoSectionHeaders"/>. List with headers for info section. The number of headers determines
		/// the maximum possible <see cref="CurrentSectionIndex"/>
		/// </summary>
		private List<ComponentInfoSectionHeader> _SectionHeaders { get; }

		/// <summary>
		/// Contains information to present in each section
		/// </summary>
		private List<IEnumerable<string>> _Info { get; set; }

		#endregion

		#region Public properties

		/// <summary>
		/// Number of sections in this info
		/// </summary>
		public int SectionsCount => _SectionHeaders.Count;

		/// <summary>
		/// Info to present from the currently section
		/// </summary>
		public IEnumerable<string> CurrentSection => _Info.Count == 0 ? Enumerable.Empty<string>() : _Info[CurrentSectionIndex];

		/// <summary>
		/// The 0-based index of currently presented info section
		/// </summary>
		public int CurrentSectionIndex
		{
			get => mCurrentSectionIndex;
			// If there are no headers defined just set 0, otherwise set the remainder from division by the number of headers
			// (to make sure that index doesn't exceed the last info section)
			set
			{
				// If there are header defined (otherwise value doesn't change and is a constant 0)
				if (_SectionHeaders.Count > 0)
				{
					// Annul selection of the previous header
					_SectionHeaders[mCurrentSectionIndex].IsSelected = false;

					// Assign new index
					mCurrentSectionIndex = value % _SectionHeaders.Count;

					// Set selection on newly selected index
					_SectionHeaders[mCurrentSectionIndex].IsSelected = true;
				}
			}
		}

		/// <summary>
		/// Headers of all info sections
		/// </summary>
		public IEnumerable<IComponentInfoSectionHeader> SectionHeaders => _SectionHeaders;

		#endregion

		#region Public methods

		/// <summary>
		/// Sets new information, if count differs from <see cref="SectionsCount"/> throws an exception
		/// </summary>
		/// <param name="info"></param>
		public void SetInfo(IEnumerable<IEnumerable<string>> info)
		{
			// Check if count is correct
			if(info.Count() != SectionsCount)
			{
				throw new ArgumentException(nameof(info) + " has a different amount of items than specified by " +
					nameof(SectionsCount));
			}

			// Create a new list for new info
			_Info = new List<IEnumerable<string>>(info);
		}

		#endregion
	}
}