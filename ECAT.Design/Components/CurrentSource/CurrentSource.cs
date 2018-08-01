using CSharpEnhanced.Maths;
using ECAT.Core;

namespace ECAT.Design
{
	/// <summary>
	/// Current source is an ideal source that can supply a chosen value of current to an arbitrary load
	/// </summary>
	public class CurrentSource : TwoTerminal, ICurrentSource
	{
		#region Constructors

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CurrentSource()
		{
			Admittance = IoC.Resolve<IDefaultValues>().CurrentSourceAdmittance;
			ProducedCurrent = IoC.Resolve<IDefaultValues>().DefaultCurrentSourceProducedCurrent;

			ProducedCurrentVar = _ProducedCurrentVarSource.Variable;
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Source of <see cref="ProducedCurrentVar"/> and a backing store for produced current of this <see cref="ICurrentSource"/>
		/// </summary>
		protected Variable.VariableSource _ProducedCurrentVarSource { get; } = new Variable.VariableSource();

		#endregion

		#region Public properties

		/// <summary>
		/// Current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public Variable ProducedCurrentVar { get; }

		/// <summary>
		/// Accessor to the current supplied by this <see cref="ICurrentSource"/>
		/// </summary>
		public double ProducedCurrent
		{
			get => _ProducedCurrentVarSource.Value.Real;
			set => _ProducedCurrentVarSource.Value = value;
		}

		#endregion
	}
}