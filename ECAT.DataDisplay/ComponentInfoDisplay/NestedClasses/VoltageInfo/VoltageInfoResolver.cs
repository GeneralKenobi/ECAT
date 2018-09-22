using ECAT.Core;
using System;
using System.Reflection;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Resolves voltage drops on components
		/// </summary>
		private class VoltageInfoResolver : ISignalInformationResolver
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="targetType"></param>
			/// <param name="terminalA"></param>
			/// <param name="terminalB"></param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public VoltageInfoResolver(Type targetType, PropertyInfo terminalA, PropertyInfo terminalB)
			{
				_TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

				// Check if given property infos are correct
				if(IsPropertyInfoValid(terminalA ?? throw new ArgumentNullException(nameof(terminalA)), targetType) &&
					IsPropertyInfoValid(terminalB ?? throw new ArgumentNullException(nameof(terminalB)), targetType))
				{
					_TerminalA = terminalA;
					_TerminalB = terminalB;
				}
				else
				{
					throw new ArgumentException("Property informations don't have a proper return type (" + typeof(ITerminal).FullName +
						") or don't match target type (" + targetType.FullName + ")");
				}
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Target type of objects for which voltage drops will be resolved (used to check if properly typed parameter was passed
			/// to <see cref="Get(IBaseComponent)"/>).
			/// </summary>
			private Type _TargetType { get; }

			/// <summary>
			/// Property info of first (reference) terminal
			/// </summary>
			private PropertyInfo _TerminalA { get; }

			/// <summary>
			/// Property info of second terminal
			/// </summary>
			private PropertyInfo _TerminalB { get; }

			#endregion

			#region Private properties

			/// <summary>
			/// Returns true if <paramref name="info"/> is a valid property info that matches the <paramref name="targetType"/>
			/// </summary>
			/// <param name="info"></param>
			/// <param name="targetType"></param>
			/// <returns></returns>
			private bool IsPropertyInfoValid(PropertyInfo info, Type targetType) =>
				info.DeclaringType == targetType && info.PropertyType == typeof(ITerminal);
				
			#endregion

			#region Public Methods

			/// <summary>
			/// Gets a voltage drop 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public ISignalInformation Get(IBaseComponent target)
			{
				// If target object's type doesn't match target type, throw an exception
				if(target.GetType() != _TargetType)
				{
					throw new ArgumentException(nameof(target) + " is of type that is not a target type for this instance " +
						"(" + nameof(target) + " is of type " + target.GetType().FullName +
						", target type is " + _TargetType.FullName + ")");
				}

				// At this point type is guaranteed to have a property given by both property infos with ITerminal return type
				var terminalAValue = (ITerminal)_TerminalA.GetValue(target);
				var terminalBValue = (ITerminal)_TerminalB.GetValue(target);

				// Get the results from provider and return its return value
				return IoC.Resolve<ISimulationResultsProvider>().Value.Voltage.Get(terminalAValue.NodeIndex, terminalBValue.NodeIndex);
			}

			#endregion
		}
	}
}