using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace EntitySystem
{
    public class DistanceHandler : MonoBehaviour
    {
        public static DistanceHandler Instance { get; private set; } = null;

        private QuadMatrix _distanceMatrix;
        private List<Entity> _entities;

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
            if (Instance != this) return;
            for (int i = 0; i < _entities.Count; i++)
            {
                for (int j = i; j < _entities.Count; j++)
                {
                    var distance = Vector2.Distance(_entities[i].gameObject.transform.position,
                        _entities[j].gameObject.transform.position);
                    _distanceMatrix.Set(i, j, distance);
                    _distanceMatrix.Set(j, i, distance);
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
            int index = GetEntityIndex(e);
            _entities.RemoveAt(index);
            _distanceMatrix.Remove(index);
        }

        public Dictionary<Entity, float> GetDistancesFor(Entity e)
        {
            int index = GetEntityIndex(e);
            var distances = _distanceMatrix.GetRow(index);
            Debug.Assert(distances.Count == _entities.Count, "Distances and registered entities do not match");
            return Enumerable.Range(0, _entities.Count).ToDictionary(i => _entities[i], i => distances[i]);
        }

        private int GetEntityIndex(Entity e)
        {
            var eId = e.GetInstanceID();
            return _entities.FindIndex(m => eId == m.GetInstanceID());
        }
    }
}