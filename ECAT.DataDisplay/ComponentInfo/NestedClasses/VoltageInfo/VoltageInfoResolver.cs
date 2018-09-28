using ECAT.Core;
using System;
using System.Reflection;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Resolves voltage drops on components
		/// </summary>
		private class VoltageInfoResolver : ResolverBase
		{
			#region Constructors

			/// <summary>
			/// Constructor for two-terminal voltage drop resolver
			/// </summary>
			/// <param name="targetType"></param>
			/// <param name="terminalA"></param>
			/// <param name="terminalB"></param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public VoltageInfoResolver(Type targetType, PropertyInfo terminalA, PropertyInfo terminalB) : base(targetType)
			{
				// Check if given property infos are correct
				if(IsPropertyInfoValid(terminalA ?? throw new ArgumentNullException(nameof(terminalA)), targetType) &&
					IsPropertyInfoValid(terminalB ?? throw new ArgumentNullException(nameof(terminalB)), targetType))
				{
					_TerminalA = terminalA;
					_TerminalB = terminalB;
				}
				else
				{
					throw new ArgumentException("Property informations doesn't have a proper return type (" + typeof(ITerminal).FullName +
						") or don't match target type (" + targetType.FullName + ")");
				}
			}

			/// <summary>
			/// Constructor for voltage drop from ground to terminal resolver
			/// </summary>
			/// <param name="targetType"></param>
			/// <param name="terminalA"></param>
			/// <param name="terminalB"></param>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public VoltageInfoResolver(Type targetType, PropertyInfo terminal) : base(targetType)
			{
				// Check if given property infos are correct
				if (IsPropertyInfoValid(terminal ?? throw new ArgumentNullException(nameof(terminal)), targetType))
				{
					_TerminalA = null;
					_TerminalB = terminal;
				}
				else
				{
					throw new ArgumentException("Property informations doesn't have a proper return type (" + typeof(ITerminal).FullName +
						") or don't match target type (" + targetType.FullName + ")");
				}
			}

			#endregion

			#region Private properties

			/// <summary>
			/// True if voltage drop should be resolved from <see cref="_TerminalB"/> to ground. False if voltage drop should be
			/// resolved from <see cref="_TerminalB"/> to <see cref="_TerminalA"/>.
			/// </summary>
			private bool _ResolveFromGround => _TerminalA == null;

			/// <summary>
			/// Property info of first (reference) terminal
			/// </summary>
			private PropertyInfo _TerminalA { get; }

			/// <summary>
			/// Property info of second terminal
			/// </summary>
			private PropertyInfo _TerminalB { get; }

			#endregion

			#region Private methods

			/// <summary>
			/// Returns true if <paramref name="info"/> is a valid property info that matches the <paramref name="targetType"/>
			/// </summary>
			/// <param name="info"></param>
			/// <param name="targetType"></param>
			/// <returns></returns>
			private bool IsPropertyInfoValid(PropertyInfo info, Type targetType) =>
				info.DeclaringType.IsAssignableFrom(targetType) && info.PropertyType == typeof(ITerminal);
				
			#endregion

			#region Public Methods

			/// <summary>
			/// Gets a voltage drop 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			protected override ISignalInformation GetSignalInformation(IBaseComponent target)
			{
				// At this point type is guaranteed to have a property given by _TerminalB with ITerminal return type
				var terminalBValue = (ITerminal)_TerminalB.GetValue(target);

				if (_ResolveFromGround)
				{
					// Resolve from ground to _TerminalB
					return IoC.Resolve<ISimulationResultsProvider>().Value.Voltage.Get(terminalBValue.NodeIndex);
				}
				else
				{
					// Resolve from _TerminalA to _TerminalB

					// At this point type is guaranteed to have a property given by _TerminalA with ITerminal return type
					var terminalAValue = (ITerminal)_TerminalA.GetValue(target);

					// Get the results from provider and return its return value
					return IoC.Resolve<ISimulationResultsProvider>().Value.Voltage.Get(terminalAValue.NodeIndex, terminalBValue.NodeIndex);
				}
			}

			#endregion
		}
	}
}