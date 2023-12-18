using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Rot
{
    public class Tile : IPathFinderTile, IReceivingInfluence
    {
        private int _vitality;
        private Dictionary<TileDirections, Tile> _adjoiningTiles;
        private Dictionary<Tile, int> _influencingTiles;
        private Location _location;

        public Vector2Int ModelPosition { get; private set; } 
        public TileTypes Type { get; private set; }
        public int Cost { get; private set; }
        public int Influence { get; private set; }

        public int Vitality
        {
            get => _vitality;
            set 
            {
                _vitality = Mathf.Clamp(value, -BaseValues.BaseVitality * 2, BaseValues.BaseVitality);
            }
        }
        public List<Tile> AdjoiningTiles => _adjoiningTiles.Values.Where(v => v != null).ToList();
        public bool IsAlive => Vitality > 0;
        public bool HasLocation => _location != null && _location.IsAlive;
        public Location Location => _location;


        List<IPathFinderTile> IPathFinderTile.AdjoiningTiles => new List<IPathFinderTile>(AdjoiningTiles.Cast<IPathFinderTile>());
        int IPathFinderTile.PathCostEstimate { get; set; }
        int IPathFinderTile.CostSoFar { get; set; }
        bool IPathFinderTile.NewPath { get; set; }
        IPathFinderTile IPathFinderTile.Previous { get; set; }


        internal Tile(Vector2Int modelPosition)
        {
            ModelPosition = modelPosition;
            _adjoiningTiles = new Dictionary<TileDirections, Tile>();
            for (int i = 0; i < 6; i++) _adjoiningTiles.Add((TileDirections)i, null);
        }

        internal void Init(TileConfig config)
        {
            Type = config.Type;
            Cost = config.PassingCost;
            Vitality = BaseValues.BaseVitality;
        }

        internal void SetAdjoiningTileAt(TileDirections direction, Tile tile)
        {
            _adjoiningTiles[direction] = tile;
            int reverseDirection = (int)direction + 3;
            if (reverseDirection < 6 && tile != null) tile.SetAdjoiningTileAt((TileDirections)reverseDirection, this);
        }

        public void ReceiveExternalInfluence(int externalInfluence)
        {
            int value = externalInfluence;
            _location?.TryGetInfluence(ref value);
            Vitality += value;
        }

        internal void ProcessExternalInfluence()
        {
            float influence = 0;

            foreach (var kvp in _influencingTiles)
                influence += (float) kvp.Key.Influence / kvp.Value;
            ReceiveExternalInfluence(Mathf.RoundToInt(influence));
        }

        internal int SetExternalInfluence() =>
            Influence = Mathf.Clamp(Vitality + (_location != null ? _location.Vitality + 3 : 0) - BaseValues.BaseVitality,
                -BaseValues.BaseVitality, 2 * BaseValues.BaseVitality);

        internal void SetInfluencingTiles()
        {
            if (_influencingTiles != null) return;

            _influencingTiles = new Dictionary<Tile, int>();

            foreach (var t in AdjoiningTiles)
                _influencingTiles.Add(t, BaseValues.InfluenceDecrement);

            foreach (var t in AdjoiningTiles)
                foreach (var tt in t.AdjoiningTiles)
                    if (tt != this && !_influencingTiles.ContainsKey(tt))
                        // how to set next level decrement?
                        _influencingTiles.Add(tt, 6 * BaseValues.InfluenceDecrement);
        }

        internal bool CheckBuildRequirements(int vitalityRequirements) =>
            Type != TileTypes.Pond;

        internal void SetLocation(Location location) => _location = location;
    }
}


