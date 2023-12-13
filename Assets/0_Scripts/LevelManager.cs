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
        [SerializeField] private UIManager _UImanager;
        [SerializeField] private GameObject _unitPrefab;

        private MapModel _model;
        private PlayerManager _playerManager;

        private float _timer = 0;
        private bool _finish = false;

        void Start()
        {
            _model = new(_config);

            _playerManager = new(_UImanager, _unitPrefab, _model);
            _playerManager.OnTurnEnd = SetNextTurn;

            _mapView.Init(_config.GetBaseMaterial);
            _mapView.DrawMap(_model.AllTiles);
            _mapView.OnTileClick = p => { _model[p.x, p.y].SetLocation(); _mapView.UpdateTiles(_model.AllTiles); };
            //_mapView.OnTileClick = p => _mapView.DrawPath(PathFinder.GetPath(_model[p.x, p.y], _model[0, 0]));
            //_mapView.OnTileClick = p => _mapView.DrawAdjoining(_model[p.x, p.y].AdjoiningTiles);

            _inputManager.OnClick = ReactOnClick;
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
            _inputManager.OnExit = () => _finish = true;

            SetNextTurn();
        }

        //private void FixedUpdate()
        //{
        //    _timer += Time.fixedDeltaTime;

        //    if (_timer > 0.9f)
        //    {
        //        _timer = 0;
        //        foreach (var t in _model.AllTiles) t.SetExternalInfluence();
        //        foreach (var t in _model.AllTiles) t.ProcessExternalInfluence();
        //        _mapView.UpdateTiles(_model.AllTiles);
        //    }
        //}

        private void ReactOnClick(Vector3 position)
        {
            _mapView.ProcessClick(_cameraView.ScreenToMapCoordinates(position));
        }

        private void SetNextTurn()
        {
            if(!_finish)
            {
                foreach (var t in _model.AllTiles) t.SetExternalInfluence();
                foreach (var t in _model.AllTiles) t.ProcessExternalInfluence();
                _mapView.UpdateTiles(_model.AllTiles);
                _playerManager.AddUnit(_model.GetRandomPosition());
                _playerManager.Act();
            }
        }
    }
}
