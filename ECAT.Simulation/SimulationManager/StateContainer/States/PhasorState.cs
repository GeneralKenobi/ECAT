using ECAT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECAT.Simulation
{
	/// <summary>
	/// Container for instantenous state of a circuit with respect to one source
	/// </summary>
	public class PhasorState : GenericState<Complex>
	{
		#region Constructor

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsIndices">Active components indices present in this instance</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(IEnumerable<int> nodeIndices, IEnumerable<int> activeComponentsIndices, ISourceDescription sourceDescription) :
			base(nodeIndices, activeComponentsIndices, sourceDescription) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodeIndices">Nodes present in this instance</param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will be given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(IEnumerable<int> nodeIndices, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(nodeIndices, Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="nodesCount">Number of nodes, indicies available in this instance will be given by a range:
		/// 0 to <paramref name="nodesCount"/></param>
		/// <param name="activeComponentsCount">Number of active components, indices available in this instance will be given by a
		/// range: 0 to <paramref name="activeComponentsCount"/> - 1</param>
		/// <param name="sourceDescription">Description of source that produced this state. Can be null - it means that it's indetermined or
		/// many sources produced this state</param>
		public PhasorState(int nodesCount, int activeComponentsCount, ISourceDescription sourceDescription) :
			this(Enumerable.Range(0, nodesCount), Enumerable.Range(0, activeComponentsCount), sourceDescription) { }

		#endregion

		#region Public methods

		/// <summary>
		/// Adds the given values to this instance. If <see cref="GenericState{T}.Potentials"/> or <see cref="GenericState{T}.Currents"/> don't
		/// have a key corresponding to index in either <paramref name="nodePotentials"/> or <paramref name="activeComponentsCurrents"/> an
		/// exception will be thrown.
		/// </summary>
		/// <param name="nodePotentials"></param>
		/// <param name="activeComponentsCurrents"></param>
		public void AddValues(Complex[] nodePotentials, Complex[] activeComponentsCurrents)
		{
			// Add node potentials
			for (int i = 0; i < nodePotentials.Length; ++i)
			{
				Potentials[i] += nodePotentials[i];
			}

			// Add active components currents
			for (int i = 0; i < activeComponentsCurrents.Length; ++i)
			{
				Currents[i] += activeComponentsCurrents[i];
			}
		}

		/// <summary>
		/// Adds the <paramref name="other"/> to this instance. If keys from <see cref="GenericState{T}.Potentials"/> or
		/// <see cref="GenericState{T}.Currents"/> are not found in this instance, an exception will be thrown.
		/// </summary>
		/// <param name="nodePotentials"></param>
		/// <param name="activeComponentsCurrents"></param>
		public void AddState(PhasorState other)
		{
			// Add node potentials
			foreach(var key in other.Potentials.Keys)
			{
				Potentials[key] += other.Potentials[key];
			}

			// Add active components currents
			foreach (var key in other.Currents.Keys)
			{
				Currents[key] += other.Currents[key];
			}
		}

		/// <summary>
		/// Creates a DC instantenous state based on phasors in this instance. Throws an exception if <see cref="GenericState{T}.SourceDescription"/>
		/// is a description of an AC source. The DC version is created by taking real parts of each phasor (for DC phasors are real numbers).
		/// </summary>
		/// <returns></returns>
		public InstantenousState ToDC()
		{
			// Check if this state can be converted to DC
			if(SourceDescription.SourceType == SourceType.ACVoltageSource)
			{
				throw new Exception("Can't convert phasor produced by AC source to DC representation");
			}

			// Create state for result
			var result = new InstantenousState(Potentials.Keys, Currents.Keys, SourceDescription);

			// Assign potentials - take real parts - phasors for DC are purely real
			foreach (var potential in Potentials)
			{
				result.Potentials[potential.Key] = potential.Value.Real;
			}

			// And currents - take real parts - phasors for DC are purely real
			foreach (var current in Currents)
			{
				result.Currents[current.Key] = current.Value.Real;
			}

			return result;
		}

		/// <summary>
		/// Creates an instantenous state for some time moment based on phasors in this instance. The time moment is defined as
		/// <paramref name="pointIndex"/> * <paramref name="timeStep"/>.
		/// </summary>
		/// <returns></returns>
		/// <param name="pointIndex">Index of the point for which the instantenous values will be calculated, indexing starts from 0</param>
		/// <param name="timeStep">Time step between 2 subsequent points</param>
		public InstantenousState ToInstantenousValues(int pointIndex, double timeStep)
		{
			// Create state for result
			var result = new InstantenousState(Potentials.Keys, Currents.Keys, SourceDescription);

			// Depending on source type
			switch (SourceDescription.SourceType)
			{
				// For AC sources
				case SourceType.ACVoltageSource:
					{
						// For each potential
						foreach (var potential in Potentials)
						{
							// Assign to result an instantenous value of a sine wave based on the phasor
							result.Potentials[potential.Key] = WaveformBuilder.SineWaveInstantenousValue(potential.Value.Magnitude,
								SourceDescription.Frequency, potential.Value.Phase, pointIndex, timeStep);
						}

						// Similarly for each current
						foreach (var current in Currents)
						{
							result.Currents[current.Key] = WaveformBuilder.SineWaveInstantenousValue(current.Value.Magnitude,
								SourceDescription.Frequency, current.Value.Phase, pointIndex, timeStep);
						}
					}
					break;

				// Phasors for DC sources are purely real - simply assign the real part of the phasor - time moment does not influence the value.
				case SourceType.DCVoltageSource:
				case SourceType.DCCurrentSource:
					{
						// For each potential
						foreach (var potential in Potentials)
						{
							result.Potentials[potential.Key] = potential.Value.Real;
						}

						// For each current
						foreach (var current in Currents)
						{
							result.Currents[current.Key] = current.Value.Real;
						}
					}
					break;
			}

			return result;
		}

		/// <summary>
		/// Builds <see cref="WaveformState"/> based on this <see cref="PhasorState"/>. Builds waveforms for each potential and current.
		/// </summary>
		/// <param name="pointsCount"></param>
		/// <param name="timeStep"></param>
		/// <returns></returns>
		public WaveformState ToWaveform(int pointsCount, double timeStep)
		{
			// Create state for result
			var result = new WaveformState(Potentials.Keys, Currents.Keys, SourceDescription);

			// Depending on source type
			switch(SourceDescription.SourceType)
			{
				// For AC sources
				case SourceType.ACVoltageSource:
					{
						// For each potential
						foreach (var potential in Potentials)
						{
							// Create sine waves based on the phasor
							result.Potentials[potential.Key] = WaveformBuilder.SineWave(potential.Value.Magnitude, SourceDescription.Frequency,
								potential.Value.Phase, pointsCount, timeStep).ToList();
						}

						// Similarly for currents
						foreach (var current in Currents)
						{
							result.Currents[current.Key] = WaveformBuilder.SineWave(current.Value.Magnitude, SourceDescription.Frequency,
								current.Value.Phase, pointsCount, timeStep).ToList();
						}
					} break;

				// For DC sources
				case SourceType.DCVoltageSource:
				case SourceType.DCCurrentSource:
					{
						// For each potential
						foreach (var potential in Potentials)
						{
							// Build constant waveforms. For DC sources phasor is a purely real number - just take the real part as value of the
							// wave.
							result.Potentials[potential.Key] = WaveformBuilder.ConstantWaveform(potential.Value.Real, pointsCount).ToList();
						}

						// Similarly for currents
						foreach (var current in Currents)
						{
							result.Currents[current.Key] = WaveformBuilder.ConstantWaveform(current.Value.Real, pointsCount).ToList();
						}
					} break;
			}

			return result;
		}

		#endregion
	}
}