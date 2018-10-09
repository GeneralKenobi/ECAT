using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ECAT.Core
{
	partial class IoC
	{
		/// <summary>
		/// Class used to select constructors for registered services that have parameters exactly matching the provided ones
		/// (first count is checked, then if all constructor parameters can have value provided by any of the provided parameters)
		/// </summary>
		private class ExactConstructorSelector : IConstructorSelector
		{
			#region Private properties

			/// <summary>
			/// Container from which services are resolved
			/// </summary>
			private IComponentContext _Context => Container;

			#endregion

			#region Public methods

			/// <summary>
			/// Selects constructor whose number of parameters matches the number of provided paramters and whose parameter all
			/// can be supplied value by any of the <paramref name="parameters"/>
			/// </summary>
			/// <param name="constructorBindings"></param>
			/// <param name="parameters"></param>
			/// <returns></returns>
			public ConstructorParameterBinding SelectConstructorBinding(ConstructorParameterBinding[] constructorBindings,
				IEnumerable<Parameter> parameters)
			{
				var parametersCount = parameters.Count();

				return constructorBindings.
					// Get all constructors that have the number of parameters equal to the number of provided parameters
					Where((constructorBinding) => constructorBinding.TargetConstructor.GetParameters().Count() == parametersCount).
					// Return the first occurance or defualt (null)
					FirstOrDefault((constructorBinding) => constructorBinding.TargetConstructor.GetParameters().
						// Check if all parameters can be supplied value
						All((constructorParameter) => parameters.
							// By any of the provided parameters
							Any((parameter) => parameter.CanSupplyValue(constructorParameter, _Context, out var provider))));
			}

			#endregion
		}
	}
}