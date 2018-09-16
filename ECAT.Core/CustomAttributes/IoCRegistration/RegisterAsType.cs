using System;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to mark classes that IoC container later locates and registers as services given by <see cref="Types"/>
	/// </summary>
	public class RegisterAsType : RegisterAsBase
	{
		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="types">Services to register the attribute's target as, can't be null</param>
		/// <exception cref="ArgumentNullException"></exception>
		public RegisterAsType(params Type[] types) : base(types) { }

		#endregion
	}
}