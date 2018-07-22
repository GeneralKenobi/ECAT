using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ECAT.Design
{
    public class Terminal : ITerminal
    {
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Terminal(IPlanePosition position)
		{
			Position = position;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
		
		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		public IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>
		/// </summary>
		public RefWrapper<double> Potential { get; set; }
	}
}