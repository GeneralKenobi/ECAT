using System.ComponentModel;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IComponentInfoSectionHeader"/>
	/// </summary>
	public class ComponentInfoSectionHeader : IComponentInfoSectionHeader
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public ComponentInfoSectionHeader(int index, string text)
		{
			Index = index;
			Text = text;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// Zero-based index
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Text to display
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// True if this header's section is selected
		/// </summary>
		public bool IsSelected { get; set; }

		#endregion
	}
}