using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    internal class MapModel
    {
        private int _radius;
        private Tile[,] _baseArray;
        private List<Tile> _allTiles;


        internal Tile this[int x, int y]
        {
            get
            {
                int i = GetBaseIndex(x);
                int j = GetBaseIndex(y);
                return _baseArray[i,j];
            }
            set
            {
                int i = GetBaseIndex(x);
                int j = GetBaseIndex(y);
                _baseArray[i,j] = value;
            }
        }
        internal List<Tile> AllTiles => _allTiles;


        internal MapModel(int radius)
        {
            _radius = radius;
            int modelSize = 2 * radius - 1;
            _baseArray = new Tile[modelSize, modelSize];
            _allTiles = new();

            CreateTiles();
        }


        private int GetBaseIndex(int centeredIndex)
        {
            var i = Mathf.Clamp(centeredIndex, 1 - _radius, _radius - 1);
            return i + _radius - 1;
        }

        private void CreateTiles()
        {
            int startIndex = 0;
            int endIndex = _radius-1;

            for (int y = 0; y < _baseArray.GetLength(0); y++)
            {
                if (y > _radius - 1) startIndex++;
                else endIndex++;

                for (int x = startIndex; x < endIndex; x++)
                {
                    _baseArray[x, y] = new(GetCenteredCoordinates(x,y));
                    SetAdjoiningTilesAt(x, y);
                    _allTiles.Add(_baseArray[x, y]);
                }
            }
        }

        private Vector2Int GetCenteredCoordinates(int baseX, int baseY) =>
            new Vector2Int(baseX - _radius + 1, baseY - _radius + 1);

        private void SetAdjoiningTilesAt(int x, int y)
        {
            Tile tile = _baseArray[x, y];
            if(y > 0 && x < _baseArray.GetLength(0)-1) tile.SetAdjoiningTileAt(TileDirections.SE, _baseArray[x + 1, y - 1]);
            if(y > 0) tile.SetAdjoiningTileAt(TileDirections.SW, _baseArray[x, y - 1]);
            if(x > 0) tile.SetAdjoiningTileAt(TileDirections.W, _baseArray[x - 1, y]);
        }
    }

    internal class Tile
    {
        private Vector2Int _modelPosition;
        private Dictionary<TileDirections, Tile> _adjoiningTiles;

        internal Vector2Int ModelPosition => _modelPosition;
        internal Tile(Vector2Int modelPosition)
        {
            _modelPosition = modelPosition;
            _adjoiningTiles = new Dictionary<TileDirections, Tile>();
            for (int i = 0; i < 6; i++) _adjoiningTiles.Add((TileDirections)i, null);
        }

        internal void SetAdjoiningTileAt(TileDirections direction, Tile tile)
        {
            _adjoiningTiles[direction] = tile;
            int reverseDirection = (int)direction + 3;
            if (reverseDirection < 6 && tile != null) tile.SetAdjoiningTileAt((TileDirections)reverseDirection, this);
        }
    }

    internal enum TileDirections
    {
        SE,
        SW,
        W,
        NW,
        NE,
        E
    }
}


