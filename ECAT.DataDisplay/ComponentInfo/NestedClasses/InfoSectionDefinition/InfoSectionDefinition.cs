﻿using ECAT.Core;
using System;
using System.Collections.Generic;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoProvider
	{
		/// <summary>
		/// Manages resolving and interpreting information about some <see cref="IBaseComponent"/>
		/// </summary>
		private class InfoSectionDefinition
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="resolver">Used to get the signal information for some <see cref="IBaseComponent"/></param>
			/// <param name="interpreter">Used to interpret the information fetched by <paramref name="resolver"/></param>
			/// <param name="index">Position of this InfoSection on display</param>
			/// <param name="header">Header for this info section</param>
			/// <exception cref="ArgumentNullException"></exception>
			public InfoSectionDefinition(ISignalInformationResolver resolver, ISignalInformationInterpreter interpreter, int index,
				string header)
			{
				Index = index;
				_Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
				_Interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
				Header = header ?? throw new ArgumentNullException(nameof(header));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Used to get the signal information for some <see cref="IBaseComponent"/>
			/// </summary>
			private ISignalInformationResolver _Resolver { get; }

			/// <summary>
			/// Used to interpret the information fetched by <see cref="_Resolver"/>
			/// </summary>
			private ISignalInformationInterpreter _Interpreter { get; }

			#endregion

			#region Public properties

			/// <summary>
			/// Position of this InfoSection on display
			/// </summary>
			public int Index { get; }

			/// <summary>
			/// Header for this info section
			/// </summary>
			public string Header { get; }

			#endregion

			#region Public methods

			/// <summary>
			/// Returns some information about some <see cref="IBaseComponent"/> (what the information is about and how it is presented
			/// depends on arguments given in constructor). If <paramref name="target"/>'s type does not match the configuration
			/// of the <see cref="ISignalInformationResolver"/> given in the constructor an exception may be thrown by it.
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public Tuple<ISignalInformation, IEnumerable<string>> GetInfo(IBaseComponent target)
			{
				var signalInfo = _Resolver.Get(target);

				return Tuple.Create(signalInfo, _Interpreter.Get(signalInfo));
			}

			#endregion
		}
	}
}