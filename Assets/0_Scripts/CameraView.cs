using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rot
{
    public interface ICamera
    {
        void MoveCamera(Vector3 direction);
        Vector3 ScreenToMapCoordinates(Vector3 screenCoordinates);
        void ZoomCamera(float deltaDistance);
    }

    public class CameraView : MonoBehaviour, ICamera
    {
        [SerializeField, Range(1, 10)] private int _moveSensitivity = 5;
        [SerializeField] private bool _inversed;

        [SerializeField, Range(1, 10)] private int _zoomSensitivity = 1;
        [SerializeField, Range(5, 20)] private int _maxZoomDistance = 10;
        private int _minZoomDistance = 1;

        private Camera _camera;
        private float _zoomDistance;
        private Plane _plane = new Plane(Vector3.up, 0);

        private void Awake()
        {
            _camera = Camera.main;
            GetZoomDistance();
        }


        public void MoveCamera(Vector3 direction)
        {
            Vector3 dir = new(direction.x, 0, direction.y);
            if (_inversed) dir *= -1;
            _camera.transform.position += dir * _moveSensitivity / 1000;
        }

        public void ZoomCamera(float deltaDistance)
        {
            float newDistance = Mathf.Clamp(_zoomDistance + deltaDistance * _zoomSensitivity / 10,
                _minZoomDistance, _maxZoomDistance);

            _camera.transform.position += _camera.transform.forward * (_zoomDistance - newDistance);
            GetZoomDistance();
        }

        public Vector3 ScreenToMapCoordinates(Vector3 screenCoordinates)
        {
            float distance;
            Vector3 _currentClick = Vector3.zero;

            Ray ray = _camera.ScreenPointToRay(screenCoordinates);
            if (_plane.Raycast(ray, out distance)) _currentClick = ray.GetPoint(distance);

            return _currentClick;
        }


        private void GetZoomDistance()
        {
            _plane.Raycast(new Ray(_camera.transform.position, _camera.transform.forward), out _zoomDistance);
        }
    }
}
