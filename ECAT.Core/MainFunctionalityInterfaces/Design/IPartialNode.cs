using CSharpEnhanced.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ECAT.Core
{
	/// <summary>
	/// Represents connection between one terminal and one wire. It is partial because it does not support connecting 3 or more
	/// components with the same node (because it's not the inteded design functionality)
	/// </summary>
    public interface IPartialNode : INotifyPropertyChanged
    {
		#region Properties

		/// <summary>
		/// The x-y position of the center of the instance on the plane
		/// </summary>
		IPlanePosition Position { get; }

		/// <summary>
		/// The radius of the partial node
		/// </summary>
		double Radius { get; }

		#endregion
	}
}