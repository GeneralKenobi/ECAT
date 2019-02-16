using ECAT.Core;
using System.Collections.Generic;

namespace ECAT.Design
{
	/// <summary>
	/// Manages <see cref="IIDLabel"/> labels - keeps track of used labels and provides default labels.
	/// </summary>
	[RegisterAsInstance(typeof(IIDLabelRegistry))]
	public class IDLabelRegistry : IIDLabelRegistry
	{
		#region Private properties

		/// <summary>
		/// Contains all currently registered labels
		/// </summary>
		private HashSet<string> _ReservedLabels { get; } = new HashSet<string>();

		/// <summary>
		/// Counter used for determining default labels
		/// </summary>
		private int _DefaultLabelCounter { get; set; } = 0;

		#endregion

		#region Public methods

		/// <summary>
		/// Tries to register <paramref name="label"/> with the registry. If it's free it will be added to the registry (reserved) and method will
		/// return true (meaning it can be used), otherwise it will return false.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public bool TryRegister(string label) => _ReservedLabels.Add(label);

		/// <summary>
		/// Returns next free default label
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public string GetNextDefault()
		{
			string label = string.Empty;

			do
			{
				// Generate label as string prefix and incremeneted counter
				label = $"Element {++_DefaultLabelCounter}";
				// If it was successfully registered stop the loop, otherwise keep going until free label is found
			} while (!TryRegister(label));

			return label;
		}

		/// <summary>
		/// Frees the given label (removes reservation). Nothing happens if the label was not reserved.
		/// </summary>
		/// <param name="label"></param>
		public void Free(string label) => _ReservedLabels.Remove(label);

		#endregion
	}
}