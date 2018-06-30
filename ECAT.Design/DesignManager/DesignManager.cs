using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ECAT.Design
{
	public class DesignManager : IDesignManager
	{
		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
