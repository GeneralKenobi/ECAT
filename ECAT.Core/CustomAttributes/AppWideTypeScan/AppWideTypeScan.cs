using System;

namespace ECAT.Core
{
	/// <summary>
	/// Base class for attributes that are used with app wide type scanning (for example for <see cref="IoC"/> registration).
	/// All types that have this attribute (or a derived attribute) defined will be passed to
	/// <see cref="Initialization.Run(System.Collections.Generic.IEnumerable{Type})"/>.
	/// Remark: types passed to <see cref="Initialization.Run(System.Collections.Generic.IEnumerable{Type})"/> are not
	/// limited only to those with <see cref="AppWideTypeScan"/> attribute and are ultimately determined by app's Main method.
	/// </summary>
	public abstract class AppWideTypeScan : Attribute { }
}