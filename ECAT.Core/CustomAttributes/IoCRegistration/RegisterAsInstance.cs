using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to mark classes that IoC container later locates and registers their instances as services given by
	/// <see cref="Types"/>. The target class has to be createable using a parameterless constructor, otherwise an
	/// exception will be thrown on the first resolve action. If it's not possible, consider a custom registration by means of a
	/// <see cref="Autofac.Module"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public class RegisterAsInstance : IoCRegistration
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="types">Services to register the attribute's target as, can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		public RegisterAsInstance(params Type[] types)
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