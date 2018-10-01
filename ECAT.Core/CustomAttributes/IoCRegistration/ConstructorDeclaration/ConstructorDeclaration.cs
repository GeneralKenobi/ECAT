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
		public ConstructorDeclaration()
		{

		}

		/// <summary>
		/// Constructor for a declaration of a constructor with one parameter
		/// </summary>
		/// <param name="firstParamName"></param>
		/// <param name="firstParamType"></param>
		public ConstructorDeclaration(string firstParamName, Type firstParamType)
		{
			DeclaradParameters = new Dictionary<string, Type>()
			{
				{firstParamName, firstParamType },
			};
		}

		/// <summary>
		/// Constructor for a declaration of a constructor with two parameters
		/// </summary>
		/// <param name="firstParamName"></param>
		/// <param name="firstParamType"></param>
		/// <param name="secondParamName"></param>
		/// <param name="secondParamType"></param>
		public ConstructorDeclaration(string firstParamName, Type firstParamType, string secondParamName, Type secondParamType)
		{
			DeclaradParameters = new Dictionary<string, Type>()
			{
				{firstParamName, firstParamType },
				{secondParamName, secondParamType},
			};
		}

		/// <summary>
		/// Constructor for a declaration of a constructor with three parameters
		/// </summary>
		/// <param name="firstParamName"></param>
		/// <param name="firstParamType"></param>
		/// <param name="secondParamName"></param>
		/// <param name="secondParamType"></param>
		/// <param name="thirdParamName"></param>
		/// <param name="thirdParamType"></param>
		public ConstructorDeclaration(string firstParamName, Type firstParamType, string secondParamName, Type secondParamType,
			string thirdParamName, Type thirdParamType)
		{
			DeclaradParameters = new Dictionary<string, Type>()
			{
				{firstParamName, firstParamType },
				{secondParamName, secondParamType},
				{thirdParamName, thirdParamType },
			};
		}

		/// <summary>
		/// Constructor for a declaration of a constructor with four parameters
		/// </summary>
		/// <param name="firstParamName"></param>
		/// <param name="firstParamType"></param>
		/// <param name="secondParamName"></param>
		/// <param name="secondParamType"></param>
		/// <param name="thirdParamName"></param>
		/// <param name="thirdParamType"></param>
		/// <param name="fourthParamName"></param>
		/// <param name="fourthParamType"></param>
		public ConstructorDeclaration(string firstParamName, Type firstParamType, string secondParamName, Type secondParamType,
			string thirdParamName, Type thirdParamType, string fourthParamName, Type fourthParamType)
		{
			DeclaradParameters = new Dictionary<string, Type>()
			{
				{firstParamName, firstParamType },
				{secondParamName, secondParamType},
				{thirdParamName, thirdParamType },
				{fourthParamName, fourthParamType},
			};
		}

		#endregion

		#region Internal properties

		/// <summary>
		/// Parameters of this constructor declaration
		/// </summary>
		internal IEnumerable<KeyValuePair<string, Type>> DeclaradParameters { get; }

		#endregion
	}
}