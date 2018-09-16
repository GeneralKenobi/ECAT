using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to mark classes that IoC container later locates and registers their instances as services given by
	/// <see cref="Types"/>. The target class has to be createable using a parameterless constructor, otherwise an
	/// exception will be thrown on the first resolve action. If it's not possible, consider a custom registration by means of a
	/// <see cref="Autofac.Module"/>
	/// </summary>
	public class RegisterAsInstance : RegisterAsBase
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="types">Services to register the attribute's target as, can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		public RegisterAsInstance(params Type[] types) : base(types) { }

		#endregion
	}
}