using ECAT.Core;
using System;

namespace ECAT.DataDisplay
{
	public partial class ComponentInfoDisplay
	{
		/// <summary>
		/// Base class for resolvers that provides means of checking type compatibility between target type and type of object passed
		/// into <see cref="ISignalInformationResolver.Get(IBaseComponent)"/>
		/// </summary>
		private abstract class ResolverBase : ISignalInformationResolver
		{
			#region Constructors

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="targetType"></param>
			/// <exception cref="ArgumentException"></exception>
			public ResolverBase(Type targetType)
			{
				_TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
			}

			#endregion

			#region Private properties

			/// <summary>
			/// Target type for objects whose information will be resolved through this instance
			/// </summary>
			private Type _TargetType { get; }

			#endregion

			#region Protected methods

			/// <summary>
			/// Returns information for <paramref name="target"/>
			/// </summary>
			/// <param name="target">Object whose information to find, its type is guaranteed to match <see cref="_TargetType"/></param>
			/// <returns></returns>
			protected abstract ISignalInformation GetSignalInformation(IBaseComponent target);

			#endregion

			#region Public methods

			/// <summary>
			/// Gets information about <paramref name="target"/>, first checks if its type is compatible with <see cref="_TargetType"/>
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public ISignalInformation Get(IBaseComponent target)
			{
				// If target object's type doesn't match target type, throw an exception
				if (target.GetType() != _TargetType)
				{
					throw new ArgumentException(nameof(target) + " is of type that is not a target type for this instance " +
						"(" + nameof(target) + " is of type " + target.GetType().FullName +
						", target type is " + _TargetType.FullName + ")");
				}

				// Call the abstract method
				return GetSignalInformation(target);
			}

			#endregion
		}
	}
}