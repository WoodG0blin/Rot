using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IInput
    {
        Action<Vector3> OnClick { set; }
        Action<Vector3> OnDrag { set; }
        Action<float> OnScroll { set; }
    }

    public class InputManager : MonoBehaviour, IInput
    {
        [SerializeField, Range(0.01f, 0.5f)] private float _clickDuration= 0.1f;
        public Action<Vector3> OnClick { get; set; }
        public Action<Vector3> OnDrag { get; set; }
        public Action<float> OnScroll { get; set; }

        private bool _mouseLeftPressed;
        private Vector3 _previousMousePosition;
        private float _pressedTimer;

        void Update()
        {
            if(Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f) OnScroll?.Invoke(-Input.mouseScrollDelta.y);

            if (_mouseLeftPressed)
            {
                _pressedTimer += Time.deltaTime;
                ProcessDrag(_previousMousePosition, Input.mousePosition);
                _previousMousePosition = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _mouseLeftPressed = true;
                _pressedTimer = 0;
                _previousMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _mouseLeftPressed = false;
                ProcessClick(Input.mousePosition);
            }

        }

        private void ProcessClick(Vector3 mousePosition)
        {
            if(_pressedTimer <= _clickDuration) OnClick?.Invoke(mousePosition);
        }

        private void ProcessDrag(Vector3 startDrag, Vector3 finishDrag)
        {
            // Drag is opposed to mouse movement
            if (_pressedTimer > _clickDuration) OnDrag?.Invoke(startDrag - finishDrag);
        }
    }
}
