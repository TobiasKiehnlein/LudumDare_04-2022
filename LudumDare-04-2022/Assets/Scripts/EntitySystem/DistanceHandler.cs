using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace EntitySystem
{
    public class DistanceHandler : MonoBehaviour
    {
        private const float UpdateInterval = .3f;
        public float LastUpdate { get; private set; } = 0;

        public static DistanceHandler Instance { get; private set; } = null;

        private QuadMatrix _distanceMatrix;
        private List<Entity> _entities;
        
        public struct DistanceInformation
        {
            public Entity Entity;
            public float Distance;

            public DistanceInformation(Entity e, float distance)
            {
                this.Entity = e;
                this.Distance = distance;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            _distanceMatrix = new QuadMatrix(0, float.PositiveInfinity);
            _entities = new List<Entity>(0);
        }

        private void Update()
        {
            if (LastUpdate < Time.time - UpdateInterval)
            {
                LastUpdate = Time.time;
                for (var i = 0; i < _entities.Count; i++)
                {
                    for (var j = i; j < _entities.Count; j++)
                    {
                        var distance = Vector2.Distance(_entities[i].gameObject.transform.position,
                            _entities[j].gameObject.transform.position);
                        _distanceMatrix.Set(i, j, distance);
                        _distanceMatrix.Set(j, i, distance);
                    }
                }
            }
        }

        public void Register(Entity e)
        {
            _entities.Add(e);
            _distanceMatrix.Insert(-1);
        }

        public void UnRegister(Entity e)
        {
            var index = GetEntityIndex(e);
            _entities.RemoveAt(index);
            _distanceMatrix.Remove(index);
        }

        public DistanceInformation[] GetDistancesFor(Entity e)
        {
            var index = GetEntityIndex(e);
            var distances = _distanceMatrix.GetRow(index);
            Debug.Assert(distances.Count == _entities.Count, "Distances and registered entities do not match");
            return _entities.Zip(distances, (e1, d) => new DistanceInformation(e1, d)).ToArray();
        }

        private int GetEntityIndex(Entity e)
        {
            var eId = e.GetInstanceID();
            return _entities.FindIndex(m => eId == m.GetInstanceID());
        }
    }
}