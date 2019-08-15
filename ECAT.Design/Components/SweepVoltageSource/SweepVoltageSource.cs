using System;
using System.Numerics;
using ECAT.Core;

namespace ECAT.Design
{
	public class SweepVoltageSource : ACVoltageSource, ISweepVoltageSource
	{
		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public SweepVoltageSource()
		{
			OutputValue = 1;
		}

		#endregion

		#region Private members

		/// <summary>
		/// Backing store for <see cref="StartFrequency"/>
		/// </summary>
		private double mStartFrequency = IoC.Resolve<IDefaultValues>().DefaultSweepStartFrequency;

		/// <summary>
		/// Backing store for <see cref="EndFrequency"/>
		/// </summary>
		private double mEndFrequency = IoC.Resolve<IDefaultValues>().DefaultSweepEndFrequency;

		#endregion

		#region Public properties

		/// <summary>
		/// Frequency from which to start the sweep
		/// </summary>
		public double StartFrequency
		{
			get => mStartFrequency;
			set
			{
				if (value < EndFrequency)
				{
					mStartFrequency = Math.Max(value, double.Epsilon);
				}
			}
		}

		/// <summary>
		/// Frequency at which to end the sweep
		/// </summary>
		public double EndFrequency
		{
			get => mEndFrequency;
			set
			{
				if (value > StartFrequency)
				{
					mEndFrequency = Math.Max(value, double.Epsilon);
				}
			}
		}

		#endregion
	}
}