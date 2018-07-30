using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	/// <summary>
	/// Converts an <see cref="ICollection{T}"/> of <see cref="IPlanePosition"/> to a <see cref="PointCollection"/> of four points
	/// defining a rectangular that contains all points in the <see cref="ICollection{T}"/>
	/// </summary>
	public class PlanePositionCollectionToOutermostPointCollectionConverter : IValueConverter
	{
		#region Public methods

		/// <summary>
		/// Converts an <see cref="ICollection{T}"/> of <see cref="IPlanePosition"/> to a <see cref="PointCollection"/> of four points
		/// defining a rectangular that contains all points in the <see cref="ICollection{T}"/>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is ICollection<IPlanePosition> collection)
			{
				// Get outermost coodrinates in the collection
				double left = collection.Min((point) => point.X);
				double right = collection.Max((point) => point.X);

				// Negate the y coordinates because UWP measures distances on y axis towards the bottom
				double top = -collection.Max((point) => point.Y);
				double bottom = -collection.Min((point) => point.Y);

				// Add the offset to all coordinates in appropriate directions (to prevent the border from being drawn
				// right on the wire, instead the wire will have a nice margin)
				left -= BorderOffset;
				right += BorderOffset;

				top -= BorderOffset;
				bottom += BorderOffset;

				// Create four points for a polygon based on it
				var pointCollection = new PointCollection()
				{
					new Point(left,top),
					new Point(right,top),
					new Point(right,bottom),
					new Point(left,bottom),
				};

				return pointCollection;
			}

			return null;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Public static properties

		/// <summary>
		/// The offset applied to the border coordinates to simulate a nice margin around the wire
		/// </summary>
		public static double BorderOffset => 20;

		#endregion
	}
}