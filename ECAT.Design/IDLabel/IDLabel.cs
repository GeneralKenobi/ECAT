using ECAT.Core;
using System;

namespace ECAT.Design
{
	/// <summary>
	/// Standard implementation of <see cref="IIDLabel"/> - provides unique indentifiers for <see cref="IBaseComponent"/>s
	/// </summary>
	[RegisterAsType(typeof(IIDLabel))]
	public class IDLabel : IIDLabel
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public IDLabel()
		{
			// Get default label string
			Label = IoC.Resolve<IIDLabelRegistry>().GetNextDefault();
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Unique label assigned to the component
		/// </summary>
		public string Label { get; private set; }

		#endregion

		#region Public methods

		/// <summary>
		/// If <paramref name="label"/> is unique then assigns it as new <see cref="Label"/> and returns true, otherwise returns false.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public bool Update(string label)
		{
			// Try to register the label
			if(IoC.Resolve<IIDLabelRegistry>().TryRegister(label))
			{
				// If successful, assign it
				Label = label;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Disposes of the label - removes it from the global registry
		/// </summary>
		public void Dispose() => IoC.Resolve<IIDLabelRegistry>().Free(Label);

		#endregion
	}
}