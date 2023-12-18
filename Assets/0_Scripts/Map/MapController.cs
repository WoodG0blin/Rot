using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public class MapController
    {
        private MapConfig _config;
        private MapModel _model;
        private MapView _view;


        public Action<Vector2Int> OnNewUnit;


        internal MapController(MapConfig config, MapView view)
        {
            _config = config;
            _model = new(_config);
            _view = view;

            _view.Init(_config.GetBaseMaterial);
            _view.DrawMap(_model.AllTiles);

        }

        public void Act()
        {
            foreach (var t in _model.AllTiles) t.SetExternalInfluence();
            foreach (var t in _model.AllTiles) t.ProcessExternalInfluence();
            _view.UpdateTiles(_model.AllTiles);
            OnNewUnit?.Invoke(_model.GetRandomPosition());
        }

        public Tile GetTile(Vector2Int modelPosition) => _model[modelPosition.x, modelPosition.y];
        public UnitView InitiateUnit() => _view.InitiateUnitView();

        internal Vector2Int WorldToMapCoordinates(Vector3 worldCoordinates) =>
            _view.WorldToMapCoordinates(worldCoordinates);
    }
}
