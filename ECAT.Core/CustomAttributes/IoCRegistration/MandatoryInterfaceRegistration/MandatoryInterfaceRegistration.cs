using System;


namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to mark interfaces that have to have an implementation registered with <see cref="IoC"/>.
	/// Using it allows to verify at start-up if all necessary services are registered instead of crashing sometime later when
	/// some service cannot be resolved.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public class MandatoryInterfaceRegistration : Attribute { }
}