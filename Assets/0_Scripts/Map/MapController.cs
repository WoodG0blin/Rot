using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class MapController
    {
        private MapConfig _config;
        private MapModel _model;
        private MapView _view;

        private List<Tile> _enemyTiles;

        internal Action<Tile> OnNewDeadTile;
        internal Action<Tile> OnRemovedDeadTile;

        internal MapController(MapConfig config, MapView view)
        {
            _config = config;
            _model = new(_config);
            _view = view;

            _view.Init(_config.GetBaseMaterial);
            _view.DrawMap(_model.AllTiles);

            _enemyTiles = new List<Tile>();

            _model.OnTileStatusChanged = ChangeTileStatus;
        }


        public void InitEnemy() => ChangeTileStatus(GetTile(_model.EnemyStartPosition));
        public void Act()
        {
            foreach (var t in _model.AllTiles) t.SetExternalInfluence();
            foreach (var t in _model.AllTiles) t.ProcessExternalInfluence();
            _view.UpdateTiles(_model.AllTiles);
        }

        public Tile GetTile(Vector2Int modelPosition) => _model[modelPosition.x, modelPosition.y];
        public void RegisterUnit(BaseUnit unit)
        {
            unit.SetView(_view.InitiateUnitView(unit is PlayerUnit));
            unit.OnPositionChange = (i, c) => MoveUnit(unit, i, c);
        }
        public void RegisterLocation(BaseLocation location) =>
            _model[location.ModelPosition.x, location.ModelPosition.y].SetLocation(location);

        internal Vector2Int WorldToMapCoordinates(Vector3 worldCoordinates) =>
            _view.WorldToMapCoordinates(worldCoordinates);


        private void MoveUnit(BaseUnit unit, Vector2Int initial, Vector2Int current)
        {
            GetTile(initial).RemoveUnit(unit);
            GetTile(current).SetUnit(unit);
        }
        private void ChangeTileStatus(Tile tile)
        {
            if (tile.IsAlive && _enemyTiles.Contains(tile))
            {
                _enemyTiles.Remove(tile);
                OnRemovedDeadTile?.Invoke(tile);
            }

            if (!tile.IsAlive && !_enemyTiles.Contains(tile))
            {
                _enemyTiles.Add(tile);
                OnNewDeadTile?.Invoke(tile);
            }
        }
    }
}
