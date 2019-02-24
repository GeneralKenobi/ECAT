using ECAT.Core;
using System;

namespace ECAT.ViewModel
{
	/// <summary>
	/// Edit view model for sources - classes that derive from <see cref="ISource"/>. Provides property to bind to that edits
	/// <see cref="ISource.OutputValue"/> as well as a header string for the output value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SourceEditViewModel<T> : SpecificComponentEditViewModel<T> where T : ISource
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="viewModel"></param>
		/// <param name="outputHeader">Can't be null</param>
		public SourceEditViewModel(ComponentViewModel viewModel, string outputHeader) : base(viewModel)
		{
			OutputHeader = outputHeader ?? throw new ArgumentNullException(nameof(outputHeader));
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The header containing information about output quantity
		/// </summary>
		public string OutputHeader { get; }

		/// <summary>
		/// Accessor to the value produced by edited source
		/// </summary>
		public double OutputValue
		{
			get => _EditedComponent.OutputValue;
			set => _EditedComponent.OutputValue = value;
		}

		#endregion
	}
}