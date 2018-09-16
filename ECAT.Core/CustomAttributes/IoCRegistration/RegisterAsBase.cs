using System;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for attributes that mark classes for registration with <see cref="IoC"/> during assembly scanning.
	/// </summary>
	public abstract class RegisterAsBase : Attribute
    {
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="types">Services to register the attribute's target as, can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		protected RegisterAsBase(params Type[] types)
		{
			Types = types ?? throw new ArgumentNullException(nameof(types));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Types to register the target type as
		/// </summary>
		public Type[] Types { get; }

		#endregion
	}
}