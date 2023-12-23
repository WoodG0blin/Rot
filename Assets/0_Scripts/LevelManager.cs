using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private MapController _mapController;
        private PlayerManager _playerManager;
        private EnemyManager _enemyManager;

        private bool _finish = false;

        void Start()
        {
            _mapController = new(_config, _mapView);

            _UImanager.Init();
            _UImanager.RequestClickInfo = RequestClickInfo;
            _UImanager.GetDamagablesAt = p => null;

            _playerManager = new(_UImanager);
            _playerManager.RegisterNewUnit = _mapController.RegisterUnit;
            _playerManager.OnTurnEnd = SetNextTurn;
            _playerManager.GetTile = _mapController.GetTile;
            _playerManager.OnNewLocation = _mapController.RegisterLocation;

            _enemyManager = new();
            _enemyManager.RegisterNewUnit = _mapController.RegisterUnit;
            _enemyManager.OnNewLocation = _mapController.RegisterLocation;
            _mapController.OnNewDeadTile = _enemyManager.AddNewTile;
            _mapController.OnRemovedDeadTile = _enemyManager.RemoveTile;

            _inputManager.OnClick = ReactOnClick;
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
            _inputManager.OnExit = () => _finish = true;

            _mapController.InitEnemy();
            SetNextTurn();
        }

        private void SetNextTurn()
        {
            if(!_finish)
            {
                _enemyManager.Act();
                _mapController.Act();
                _playerManager.Act();
            }
        }

        private async Task<ClickInfo> RequestClickInfo(Action cancellation)
        {
            var info = await _inputManager.GetClick(cancellation);
            if (info != null)
            {
                info.WorldCoordinates = _cameraView.ScreenToMapCoordinates(info.ScreenCoordinates);
                info.MapCoordinates = _mapController.WorldToMapCoordinates(info.WorldCoordinates);
            }
            return info;
        }
        private void ReactOnClick(Vector3 position) { }
    }
}
