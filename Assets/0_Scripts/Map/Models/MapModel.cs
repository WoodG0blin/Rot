using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rot
{
    internal class MapModel
    {
        private MapConfig _config;

        private int _radius;
        private Tile[,] _baseArray;
        private List<Tile> _allTiles;

        internal Vector2Int PlayerStartPosition { get; private set; }
        internal Vector2Int EnemyStartPosition { get; private set; }

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
        internal Action<Tile> OnTileStatusChanged { get; set; }

        internal MapModel(MapConfig config)
        {
            _config = config;

            _radius = _config.Radius;
            int modelSize = 2 * _radius - 1;
            _baseArray = new Tile[modelSize, modelSize];
            _allTiles = new();

            CreateTiles();
            foreach(var t in AllTiles) t.SetInfluencingTiles();
            SetInitialPositions();

            PathFinder.Init(this);
        }
        internal Vector2Int GetRandomPosition() => AllTiles[UnityEngine.Random.Range(0, AllTiles.Count)].ModelPosition;

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
                    var t = new Tile(GetCenteredCoordinates(x, y));
                    _baseArray[x, y] = t;
                    t.Init(_config.GetTileConfig(_config.GetRandomTileType()));
                    SetAdjoiningTilesAt(x, y);
                    t.OnStatusChanged = () => OnTileStatusChanged?.Invoke(t);
                    _allTiles.Add(t);
                }
            }
        }

        private Vector2Int GetCenteredCoordinates(int baseX, int baseY) =>
            new Vector2Int(baseX - _radius + 1, baseY - _radius + 1);

        private void SetAdjoiningTilesAt(int x, int y)
        {
            Tile tile = _baseArray[x, y];
            if(y > 0) tile.SetAdjoiningTileAt(TileDirections.SE, _baseArray[x, y - 1]);
            if(y > 0 && x > 0) tile.SetAdjoiningTileAt(TileDirections.SW, _baseArray[x-1, y - 1]);
            if(x > 0) tile.SetAdjoiningTileAt(TileDirections.W, _baseArray[x - 1, y]);
        }

        private void SetInitialPositions()
        {
            PlayerStartPosition = new(0, 0);

            int enemyStart = UnityEngine.Random.Range(0, AllTiles.Count);

            while (Vector2.SqrMagnitude(AllTiles[enemyStart].ModelPosition - PlayerStartPosition) < _radius)
                enemyStart = UnityEngine.Random.Range(0, AllTiles.Count);

            EnemyStartPosition = AllTiles[enemyStart].ModelPosition;
            int influence = -BaseValues.BaseVitality;
            AllTiles[enemyStart].TryGetInfluence(ref influence);
        }
    }
}


