namespace ECAT.Simulation
{
	/// <summary>
	/// Contains information about node indices for op-amp
	/// </summary>
	public class OpAmpNodeInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="output"></param>
		/// <param name="nonInvertingInput"></param>
		/// <param name="invertingInput"></param>
		public OpAmpNodeInfo(int output, int nonInvertingInput, int invertingInput)
		{
			Output = output;
			NonInvertingInput = nonInvertingInput;
			InvertingInput = invertingInput;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Output node of the op-amp
		/// </summary>
		public int Output { get; }

		/// <summary>
		/// Non-inverting input of the op-amp (+ terminal)
		/// </summary>
		public int NonInvertingInput { get; }

		/// <summary>
		/// Inverting input of the op-amp (- terminal)
		/// </summary>
		public int InvertingInput { get; }
		
		#endregion
	}
}