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
			public VoltageInfoResolver(Type targetType, PropertyInfo terminalA, PropertyInfo terminalB)
			{
				_TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
				_TerminalA = terminalA ?? throw new ArgumentNullException(nameof(terminalA));
				_TerminalB = terminalB ?? throw new ArgumentNullException(nameof(terminalB));
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