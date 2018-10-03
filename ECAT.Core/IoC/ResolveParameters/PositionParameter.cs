using Autofac;
using Autofac.Core;
using System;
using System.Reflection;

namespace ECAT.Core
{
	/// <summary>
	/// Parameter that matches only if its declared position matches the constructor parameter's position and if provided value
	/// is assignable to the constructor parameter's type.
	/// </summary>
	public class PositionParameter : Parameter
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="position">Position of the target parameter in the constructor, 0-based indexing</param>
		/// <param name="value">Value to provide to the constructor</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public PositionParameter(int position, object value)
		{
			Position = position >= 0 ? position : throw new ArgumentOutOfRangeException(nameof(position) + " can't be negative");
			Value = value;
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Position of the target parameter in the constructor, 0-based indexing
		/// </summary>
		private int Position { get; }

		/// <summary>
		/// Value to provide to the constructor
		/// </summary>
		private object Value { get; }

		#endregion

		#region Private methods

		/// <summary>
		/// Method used to provide value to the constructor's matching parameter
		/// </summary>
		/// <returns></returns>
		private object ValueProvider() => Value;

		#endregion

		#region Public methods

		/// <summary>
		/// Returns true if the parameter is able to provide a value to a particular site.
		/// </summary>
		/// <param name="pi">Constructor, method, or property-mutator parameter.</param>
		/// <param name="context">The component context in which the value is being provided.</param>
		/// <param name="valueProvider">If the result is true, the valueProvider parameter will be set to a function
		/// that will lazily retrieve the parameter value. If the result is false, will be set to null.</param>
		/// <returns>True if a value can be supplied; otherwise, false.</returns>
		public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
		{
			// If the positions match and the the value type is assignable to parameter
			if(pi.Position == Position && pi.ParameterType.IsAssignableFrom(Value.GetType()))
			{
				// Assign the retrieving function and return success
				valueProvider = ValueProvider;
				return true;
			}
			else
			{
				// Assign null to retrieving function and return failure
				valueProvider = null;
				return false;
			}
		}

		#endregion
	}
}