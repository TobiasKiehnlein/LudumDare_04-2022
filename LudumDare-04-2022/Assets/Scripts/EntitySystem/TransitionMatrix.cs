using System;
using System.Linq;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace EntitySystem
{
    public class TransitionMatrix<T> where T : Enum
    {
        private readonly int entries;
        private Matrix _baseMatrix;
        private float[] _multipliers;
        private const float MaxMultiplier = 3.0f;

        public TransitionMatrix()
        {
            entries = Enum.GetValues(typeof(T)).Length;
            _baseMatrix = new QuadMatrix(entries, 1);
            _multipliers = Enumerable.Repeat(1f, entries).ToArray();
            Debug.Assert(_baseMatrix.N == _multipliers.Length);
        }

        public void SetWeight(T fromState, T toState, float newWeight)
        {
            _baseMatrix.Set(fromState.ToInt(), toState.ToInt(), Mathf.Max(newWeight, 0));
        }

        // Amount will be added to the general multiplier of the weight of the state
        public void BoostState(T state, float amount)
        {
            var index = state.ToInt();
            _multipliers[index] = Mathf.Clamp(_multipliers[index] + amount, 0, MaxMultiplier);
        }

        // Lerps the multipliers back to 0, Use time.deltaTime * Speed as input
        public void ReduceMultipliers(float t)
        {
            _multipliers = _multipliers.Select(w => Mathf.Lerp(w, 1, t)).ToArray();
        }

        public T GetNextState(T currentState)
        {
            var weights = _baseMatrix.GetRow(currentState.ToInt()).Select((w, i) => w * _multipliers[i]);
            var weightSum = weights.Sum();
            var normalWeights = weights.Select(w => w / weightSum);
            var weightCounter = 0f;
            var rnd = Random.Range(0f, 1f);
            foreach (var (w, i) in normalWeights.Select((w, i) => (w, i)))
            {
                weightCounter += w;
                if (weightCounter > rnd)
                {
                    return i.ToEnum<T>();
                }
            }

            return 1.ToEnum<T>();
        }
    }
}