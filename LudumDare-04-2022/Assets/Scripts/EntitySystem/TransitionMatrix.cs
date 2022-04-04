using System;
using System.Linq;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace EntitySystem
{
    public class TransitionMatrix<T> where T : Enum
    {
        private static readonly int Entries;
        private static readonly EnumMatrix BaseMatrix = null;
        private float[] _multipliers;
        private const float MaxMultiplier = 3.0f;

        private class EnumMatrix
        {
            public Matrix Matrix { get; private set; }

            public EnumMatrix()
            {
                Matrix = new QuadMatrix(Entries, 1);
                if (typeof(T) == typeof(Death.Mood))
                {
                    SetupDeath();
                }
                else if (typeof(T) == typeof(Human.Mood))
                {
                    SetupHuman();
                }
                else
                {
                    throw new ArgumentException($"TransitionMatrix: No supported setup for enum {nameof(T)}");
                }
            }
            
            private void SetWeight<TU>(TU fromState, TU toState, float newWeight) where TU: Enum
            {
                Matrix.Set(fromState.ToInt(), toState.ToInt(), Mathf.Max(newWeight, 0));
            }

            private void SetupHuman()
            {
                SetWeight(Human.Mood.Chilling, Human.Mood.Chilling, 20);
                SetWeight(Human.Mood.Chilling, Human.Mood.Crowdy, 2);
                SetWeight(Human.Mood.Chilling, Human.Mood.AntiCrowdy, 3);
                SetWeight(Human.Mood.Chilling, Human.Mood.Jogging, 1);
                SetWeight(Human.Mood.Chilling, Human.Mood.Afraid, 0.5f);

                SetWeight(Human.Mood.Crowdy, Human.Mood.Chilling, 5);
                SetWeight(Human.Mood.Crowdy, Human.Mood.Crowdy, 20);
                SetWeight(Human.Mood.Crowdy, Human.Mood.AntiCrowdy, 5);
                SetWeight(Human.Mood.Crowdy, Human.Mood.Jogging, 2);
                SetWeight(Human.Mood.Crowdy, Human.Mood.Afraid, 0.1f);

                SetWeight(Human.Mood.AntiCrowdy, Human.Mood.Chilling, 4);
                SetWeight(Human.Mood.AntiCrowdy, Human.Mood.Crowdy, 1);
                SetWeight(Human.Mood.AntiCrowdy, Human.Mood.AntiCrowdy, 10);
                SetWeight(Human.Mood.AntiCrowdy, Human.Mood.Jogging, 6);
                SetWeight(Human.Mood.AntiCrowdy, Human.Mood.Afraid, 1);

                SetWeight(Human.Mood.Jogging, Human.Mood.Chilling, 4);
                SetWeight(Human.Mood.Jogging, Human.Mood.Crowdy, 4);
                SetWeight(Human.Mood.Jogging, Human.Mood.AntiCrowdy, 1);
                SetWeight(Human.Mood.Jogging, Human.Mood.Jogging, 20);
                SetWeight(Human.Mood.Jogging, Human.Mood.Afraid, 0.5f);

                SetWeight(Human.Mood.Afraid, Human.Mood.Chilling, 1);
                SetWeight(Human.Mood.Afraid, Human.Mood.Crowdy, 5);
                SetWeight(Human.Mood.Afraid, Human.Mood.AntiCrowdy, 2);
                SetWeight(Human.Mood.Afraid, Human.Mood.Jogging, 6);
                SetWeight(Human.Mood.Afraid, Human.Mood.Afraid, 15);
            }

            private void SetupDeath()
            {
                SetWeight(Death.Mood.Normal, Death.Mood.Normal, 15);
                SetWeight(Death.Mood.Normal, Death.Mood.Aggressive, 5);
                SetWeight(Death.Mood.Normal, Death.Mood.KillingSpree, 1);
                SetWeight(Death.Mood.Normal, Death.Mood.Starving, 3);
                SetWeight(Death.Mood.Normal, Death.Mood.Killing, 0);
            
                SetWeight(Death.Mood.Aggressive, Death.Mood.Normal, 8);
                SetWeight(Death.Mood.Aggressive, Death.Mood.Aggressive, 10);
                SetWeight(Death.Mood.Aggressive, Death.Mood.KillingSpree, 5);
                SetWeight(Death.Mood.Aggressive, Death.Mood.Starving, 8);
                SetWeight(Death.Mood.Aggressive, Death.Mood.Killing, 0);
            
                SetWeight(Death.Mood.KillingSpree, Death.Mood.Normal, 10);
                SetWeight(Death.Mood.KillingSpree, Death.Mood.Aggressive, 3);
                SetWeight(Death.Mood.KillingSpree, Death.Mood.KillingSpree, 8);
                SetWeight(Death.Mood.KillingSpree, Death.Mood.Starving, 0.5f);
                SetWeight(Death.Mood.KillingSpree, Death.Mood.Killing, 0);
            
                SetWeight(Death.Mood.Starving, Death.Mood.Normal, 5);
                SetWeight(Death.Mood.Starving, Death.Mood.Aggressive, 15);
                SetWeight(Death.Mood.Starving, Death.Mood.KillingSpree, 1);
                SetWeight(Death.Mood.Starving, Death.Mood.Starving, 10);
                SetWeight(Death.Mood.Starving, Death.Mood.Killing, 0);
                
                SetWeight(Death.Mood.Killing, Death.Mood.Normal, 0);
                SetWeight(Death.Mood.Killing, Death.Mood.Aggressive, 0);
                SetWeight(Death.Mood.Killing, Death.Mood.KillingSpree, 0);
                SetWeight(Death.Mood.Killing, Death.Mood.Starving, 0);
                SetWeight(Death.Mood.Killing, Death.Mood.Killing, 1); // Death is set back to other state via override
            }
        }

        static TransitionMatrix()
        {
            Entries = Enum.GetValues(typeof(T)).Length;
            BaseMatrix = new EnumMatrix();
        }
        
        public TransitionMatrix()
        {
            _multipliers = Enumerable.Repeat(1f, Entries).ToArray();
            Debug.Assert(BaseMatrix.Matrix.N == _multipliers.Length);
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
            var weights = BaseMatrix.Matrix.GetRow(currentState.ToInt()).Select((w, i) => w * _multipliers[i]);
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