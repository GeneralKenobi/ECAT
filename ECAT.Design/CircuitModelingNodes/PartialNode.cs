using CSharpEnhanced.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Class used to connect <see cref="BaseComponent"/> with
	/// </summary>
	public class PartialNode : INotifyPropertyChanged
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Public properties

		/// <summary>
		/// The coordinate of this PartialNode
		/// </summary>
		public cdouble Coord { get; set; }

		#endregion
	}
}