using System.ComponentModel;
using ECAT.Core;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Standard implementation of <see cref="IComponentInfoSectionHeader"/>
		/// </summary>
		private class ComponentInfoSectionHeader : IComponentInfoSectionHeader
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="index">Zero-based index of the header from beginning (whole enumeration for a given component should be
			/// strictly increasing by 1)</param>
			/// <param name="text"></param>
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
			/// Zero-based index of the header from beginning (whole enumeration for a given component should be strictly increasing by 1)
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
}