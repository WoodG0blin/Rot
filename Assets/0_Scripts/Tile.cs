using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rot
{
    internal class Tile : IPathFinderTile
    {
        private Vector2Int _modelPosition;
        private Dictionary<TileDirections, Tile> _adjoiningTiles;


        public Vector2Int ModelPosition => _modelPosition; 
        public List<Tile> AdjoiningTiles => _adjoiningTiles.Values.ToList();
        public int Cost { get; private set; }

        List<IPathFinderTile> IPathFinderTile.AdjoiningTiles => new List<IPathFinderTile>(AdjoiningTiles.Cast<IPathFinderTile>());
        int IPathFinderTile.PathCostEstimate { get; set; }
        int IPathFinderTile.CostSoFar { get; set; }
        bool IPathFinderTile.NewPath { get; set; }
        IPathFinderTile IPathFinderTile.Previous { get; set; }


        internal Tile(Vector2Int modelPosition)
        {
            _modelPosition = modelPosition;
            _adjoiningTiles = new Dictionary<TileDirections, Tile>();
            for (int i = 0; i < 6; i++) _adjoiningTiles.Add((TileDirections)i, null);
            Cost = 1;
        }

        internal void SetAdjoiningTileAt(TileDirections direction, Tile tile)
        {
            _adjoiningTiles[direction] = tile;
            int reverseDirection = (int)direction + 3;
            if (reverseDirection < 6 && tile != null) tile.SetAdjoiningTileAt((TileDirections)reverseDirection, this);
        }
    }
}


