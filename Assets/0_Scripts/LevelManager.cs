using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapConfig _config;
        [SerializeField] private MapView _mapView;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private CameraView _cameraView;

        void Start()
        {
            MapModel model = new(_config);

            _mapView.Init(_config.GetBaseMaterial);
            _mapView.DrawMap(model.AllTiles);
            _mapView.OnTileClick = p => _mapView.DrawPath(PathFinder.GetPath(model[p.x, p.y], model[0, 0]));
            //_mapView.OnTileClick = p => _mapView.DrawAdjoining(model[p.x, p.y].AdjoiningTiles);

            _inputManager.OnClick = ReactOnClick;
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
        }

        private void ReactOnClick(Vector3 position)
        {
            _mapView.ProcessClick(_cameraView.ScreenToMapCoordinates(position));
        }
    }
}
