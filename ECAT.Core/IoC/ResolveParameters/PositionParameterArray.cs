using System.Collections.Generic;
using System.Linq;

namespace ECAT.Core
{
	/// <summary>
	/// Helprs constructing an array of parameters in a quicker manner. Parameters' position is exact to their order of addition to
	/// the array.
	/// </summary>
	public class PositionParameterArray
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		private PositionParameterArray() { }

		#endregion

		#region Private properties

		/// <summary>
		/// Values stored in particular order
		/// </summary>
		private List<object> _StoredValues { get; } = new List<object>();

		#endregion

		#region Public methods

		/// <summary>
		/// Adds a new value to the end of the value array
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public PositionParameterArray Add(object Value)
		{
			_StoredValues.Add(Value);

			return this;
		}

		/// <summary>
		/// Returns an array of parameters
		/// </summary>
		/// <returns></returns>
		public PositionParameter[] GetParameters()
		{
			int counter = 0;

			return _StoredValues.Select((value) => new PositionParameter(counter++, value)).ToArray();
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Creates a new instance of <see cref="PositionParameterArray"/>
		/// </summary>
		/// <returns></returns>
		public static PositionParameterArray Create() => new PositionParameterArray();

		#endregion

		#region Operators

		/// <summary>
		/// Uses <see cref="GetParameters"/> method to convert this instance to parameter array
		/// </summary>
		/// <param name="array"></param>
		public static implicit operator PositionParameter[] (PositionParameterArray array) => array.GetParameters();

		#endregion
	}
}