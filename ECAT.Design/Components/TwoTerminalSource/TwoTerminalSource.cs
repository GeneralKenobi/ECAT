using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Base class for <see cref="ITwoTerminalSource"/>s
	/// </summary>
	public abstract class TwoTerminalSource : TwoTerminal, ISource
	{
		#region Private members

		/// <summary>
		/// Backing store for <see cref="OutputValue"/>
		/// </summary>
		private double mOutputValue;

		#endregion

		#region Protected properties

		/// <summary>
		/// Backing store for <see cref="Description"/>
		/// </summary>
		protected SourceDescription _Description { get; } = new SourceDescription();

		#endregion

		#region Public properties

		/// <summary>
		/// Output value of the source (voltage, current, etc.)
		/// </summary>
		public double OutputValue
		{
			get => mOutputValue;
			set
			{
				mOutputValue = value;
				_Description.OutputValue = value;
			}
		}

		/// <summary>
		/// Description of this <see cref="ISource"/>
		/// </summary>
		public ISourceDescription Description => _Description;

		#endregion
	}
}