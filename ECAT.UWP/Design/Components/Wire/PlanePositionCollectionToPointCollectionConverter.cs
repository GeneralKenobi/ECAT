using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ECAT.UWP
{
	public class PlanePositionCollectionToPointCollectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if(value is ICollection<PlanePosition> collection)
			{
				var pointCollection = new PointCollection();

				var transformedCollection = collection.Select((pos) => new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)));

				foreach(var item in transformedCollection)
				{
					pointCollection.Add(item);
				}

				return pointCollection;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
