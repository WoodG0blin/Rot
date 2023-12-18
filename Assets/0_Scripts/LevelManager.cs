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

        private bool _finish = false;

        void Start()
        {
            _mapController = new(_config, _mapView);

            _UImanager.Init();
            _UImanager.RequestClickInfo = RequestClickInfo;
            _UImanager.GetDamagablesAt = p => null;
            _UImanager.GetTile = _mapController.GetTile;

            _playerManager = new(_UImanager);
            _playerManager.GetPlayerUnitView = _mapController.InitiateUnit;
            _playerManager.OnTurnEnd = SetNextTurn;

            _mapController.OnNewUnit = _playerManager.AddUnit;

            _inputManager.OnClick = ReactOnClick;
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
            _inputManager.OnExit = () => _finish = true;

            SetNextTurn();
        }

        private void SetNextTurn()
        {
            if(!_finish)
            {
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
