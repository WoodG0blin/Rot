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

        private MapModel _model;

        private float _timer = 0;

        void Start()
        {
            _model = new(_config);

            _mapView.Init(_config.GetBaseMaterial);
            _mapView.DrawMap(_model.AllTiles);
            _mapView.OnTileClick = p => { _model[p.x, p.y].SetLocation(); _mapView.UpdateTiles(_model.AllTiles); };
            //_mapView.OnTileClick = p => _mapView.DrawPath(PathFinder.GetPath(_model[p.x, p.y], _model[0, 0]));
            //_mapView.OnTileClick = p => _mapView.DrawAdjoining(_model[p.x, p.y].AdjoiningTiles);

            _inputManager.OnClick = ReactOnClick;
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
        }

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer > 0.9f)
            {
                _timer = 0;
                foreach (var t in _model.AllTiles) t.SetExternalInfluence();
                foreach (var t in _model.AllTiles) t.ProcessExternalInfluence();
                _mapView.UpdateTiles(_model.AllTiles);
            }
        }

        private void ReactOnClick(Vector3 position)
        {
            _mapView.ProcessClick(_cameraView.ScreenToMapCoordinates(position));
        }
    }
}
