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
        }

        public Tile GetTile(Vector2Int modelPosition) => _model[modelPosition.x, modelPosition.y];
        public void RegisterUnit(PlayerUnit unit)
        {
            unit.SetView(_view.InitiateUnitView());
        }
        public void RegisterLocation(Location location) =>
            _model[location.ModelPosition.x, location.ModelPosition.y].SetLocation(location);

        internal Vector2Int WorldToMapCoordinates(Vector3 worldCoordinates) =>
            _view.WorldToMapCoordinates(worldCoordinates);
    }
}
