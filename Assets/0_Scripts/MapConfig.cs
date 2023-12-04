using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Rot
{
    [CreateAssetMenu(fileName = nameof(MapConfig), menuName = "Configs/" + nameof(MapConfig), order = 0)]
    internal class MapConfig : ScriptableObject
    {
        [field: SerializeField] public int Radius { get; private set; }
        [SerializeField] private TileConfig[] _tileConfigs;

        private int _sumOfProbabilities = 0;
        private Dictionary<int, TileTypes> _probabilities;

        public TileTypes GetRandomTileType()
        {
            if (_probabilities == null) SetProbabilities();
            int rnd = UnityEngine.Random.Range(0, _sumOfProbabilities);

            foreach (var kvp in _probabilities)
            {
                if (kvp.Key > rnd) return kvp.Value;
                //Debug.Log($"Checking {kvp.Value}, {kvp.Key} with random value {rnd}");
            }

            return _probabilities.Last().Value;
        }

        private void SetProbabilities()
        {
            if (_tileConfigs == null) return;

            _probabilities = new();
            _sumOfProbabilities = 0;

            foreach(var t in _tileConfigs)
            {
                _sumOfProbabilities += t.Probability;
                _probabilities.Add(_sumOfProbabilities, t.Type);
            }
        }

        public TileConfig GetTileConfig(TileTypes type) => _tileConfigs.Where(c => c.Type == type).FirstOrDefault();
        public Material GetBaseMaterial(TileTypes type)
        {
            var c = _tileConfigs.Where(c => c.Type == type).First();
            return c == null ? null : c.BaseMaterial;
        }
    }
}
