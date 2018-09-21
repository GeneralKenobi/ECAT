using System;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for attributes that are used as marks for IoC registration with type scanning.
	/// All types that have this attribute (or a derived attribute) defined will be passed to
	/// <see cref="IoC.Build(System.Collections.Generic.IEnumerable{Type})"/> by appropriate start-up routine
	/// </summary>
	public abstract class IoCRegistration : Attribute { }
}