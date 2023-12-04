using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapView _mapView;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private CameraView _cameraView;

        void Start()
        {
            MapModel model = new(4);

            _mapView.DrawMap(model.AllTiles);

            _inputManager.OnClick = p => _mapView.ProcesClick(_cameraView.ScreenToMapCoordinates(p));
            _inputManager.OnDrag = _cameraView.MoveCamera;
            _inputManager.OnScroll = _cameraView.ZoomCamera;
        }
    }
}
