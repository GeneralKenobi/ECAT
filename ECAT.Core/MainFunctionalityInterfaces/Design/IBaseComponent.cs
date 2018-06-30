using CSharpEnhanced.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Inteface for base class for all components
	/// </summary>
    public interface IBaseComponent : INotifyPropertyChanged, IDisposable
    {
		#region Properties

		/// <summary>
		/// The center of the component
		/// </summary>
		cdouble Center { get; set; }

		#endregion
	}
}
