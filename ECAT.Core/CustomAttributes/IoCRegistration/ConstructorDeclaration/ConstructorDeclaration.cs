using System;
using System.Collections.Generic;

namespace ECAT.Core
{
	/// <summary>
	/// Attribute used to mark what constructors should be available (this is enforceable by <see cref="IoC"/>) on a service
	/// implementation. It can be considered as a guarantee that resolving a service with arguments matching this
	/// <see cref="ConstructorDeclaration"/> won't fail due to no matching constructor. Every service is guaranteed to be resolvable
	/// without any parameters unless <see cref="ConstructorDeclaration"/>s are specified and a parameterless
	/// <see cref="ConstructorDeclaration"/> is not one of them (similarly to standard constructors).
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public class ConstructorDeclaration : Attribute
	{
		#region Constructors

		/// <summary>
		/// Constructor for a declaration of a paremeterless constructor
		/// </summary>
		public ConstructorDeclaration() : this(Array.Empty<Type>()) { }

		/// <summary>
		/// Constructor for a declaration of a constructor with one parameter
		/// </summary>
		/// <param name="type">Type of the single parameter</param>
		/// <param name="description">Description of the argument</param>
		public ConstructorDeclaration(Type type, string description) : this(new Type[] { type }, description) { }

		/// <summary>
		/// Constructor for a declaration of a constructor with parameters
		/// </summary>
		/// <param name="types">Types of parameters that should appear in the constructor (in that particular order)</param>
		/// <param name="descriptions">Descriptions of what each parameter represents (should represent). It's only stored in the
		/// metadata for interface providers and consumers</param>
		public ConstructorDeclaration(Type[] types, params string[] descriptions)
		{
			Parameters = types ?? throw new ArgumentNullException(nameof(types));
		}

		#endregion

		#region Internal properties

		/// <summary>
		/// Parameters defining this constructor declaration (in that particular order)
		/// </summary>
		internal IEnumerable<Type> Parameters { get; }

		/// <summary>
		/// If true, missing contructor parameters will be resolved by IoC container. False by default
		/// </summary>
		internal bool Autowired { get; set; }

		#endregion
	}
}