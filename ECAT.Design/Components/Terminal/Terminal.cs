using CSharpEnhanced.CoreClasses;
using ECAT.Core;
using System;
using System.ComponentModel;

namespace ECAT.Design
{
	/// <summary>
	/// Implementation of the <see cref="ITerminal"/> interface; terminal is an ending of an <see cref="IBaseComponent"/> that
	/// can be connected. It has an <see cref="IPlanePosition"/> and can be assigned an <see cref="RefWrapperPropertyChanged{T}"/>
	/// to have a real-time updated value of potential at the terminal
	/// </summary>
	public class Terminal : ITerminal
    {
		#region Constructors

		/// <summary>
		/// Default constructor taking position of the terminal as a parameter
		/// </summary>
		public Terminal(IPlanePosition position) => Position = position;

		/// <summary>
		/// Constructor taking position of the terminal as a parameter as well as a callback for when the value of the potential at
		/// the terminal changes (subscribes to <see cref="PotentialValueChanged"/> with it)
		/// </summary>
		/// <param name="position"></param>
		/// <param name="potentialValueChangedCallback"></param>
		public Terminal(IPlanePosition position, EventHandler potentialValueChangedCallback) : this(position) =>
			PotentialValueChanged += potentialValueChangedCallback;

		#endregion

		#region Events

		/// <summary>
		/// Event fired whenever a property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired whenever the value of the potential changes
		/// </summary>
		public EventHandler PotentialValueChanged { get; set; }

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="Potential"/>
		/// </summary>
		private RefWrapperPropertyChanged<double> mPotential;

		#endregion

		#region Public properties

		/// <summary>
		/// Position of the terminal on the design area used to connect the terminals
		/// </summary>
		public IPlanePosition Position { get; }

		/// <summary>
		/// Reference to potential at <see cref="INode"/> that is associated with this <see cref="ITerminal"/>
		/// </summary>
		public RefWrapperPropertyChanged<double> Potential
		{
			get => mPotential;
			set
			{
				// If the new value is different
				if (mPotential != value)
				{
					// If the old value wasn't null
					if (mPotential != null)
					{
						// Unsubsribe from its property changed event
						mPotential.PropertyChanged -= PropagatePotenialValueChanged;
					}

					// Assgin the new value
					mPotential = value;

					// If it's not null
					if(mPotential != null)
					{
						// Subscribe to its property changed event
						mPotential.PropertyChanged += PropagatePotenialValueChanged;
					}

					// Notify about possible change in value
					InvokePotentialValueChanged();
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Invokes the potential value changed event
		/// </summary>
		private void InvokePotentialValueChanged() => PotentialValueChanged?.Invoke(this, EventArgs.Empty);

		/// <summary>
		/// Propagates the property changed event of <see cref="Potential"/> through <see cref="PotentialValueChanged"/>.
		/// There's only one property on <see cref="RefWrapperPropertyChanged{T}"/> so does not check for the name of the property
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PropagatePotenialValueChanged(object sender, PropertyChangedEventArgs e) => InvokePotentialValueChanged();		

		#endregion
	}
}