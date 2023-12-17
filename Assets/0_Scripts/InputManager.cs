using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rot
{
    public interface IInput
    {
        Action<Vector3> OnClick { set; }
        Action<Vector3> OnDrag { set; }
        Action<float> OnScroll { set; }
        Action OnExit { set; }
    }

    public class InputManager : AwaitableView, IInput
    {
        [SerializeField, Range(0.01f, 0.5f)] private float _clickDuration= 0.1f;
        public Action<Vector3> OnClick { get; set; }
        public Action<Vector3> OnDrag { get; set; }
        public Action<float> OnScroll { get; set; }
        public Action OnExit { get; set; }

        private bool _mouseLeftPressed;
        private Vector3 _previousMousePosition;
        private float _pressedTimer;

        private ClickInfo _currentClickInfo;

        private bool _canAwaitLeftClick = false;

        void Update()
        {
            if(Input.GetKey(KeyCode.Escape)) OnExit?.Invoke();

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
                ProcessLeftClick(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(1)) SetClickInfo(Input.mousePosition, false);
        }


        public async Task<ClickInfo> GetClick(Action cancellation)
        {
            _currentClickInfo = null;
            cancellation += () => currentAwaiter?.Finish();

            _canAwaitLeftClick = false;
            await this;

            cancellation -= () => currentAwaiter?.Finish();
            return _currentClickInfo;
        }

        private void ProcessLeftClick(Vector3 mousePosition)
        {
            if (_pressedTimer <= _clickDuration)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    SetClickInfo(mousePosition, true);
                    OnClick?.Invoke(mousePosition);
                }
                if (_canAwaitLeftClick) currentAwaiter?.Finish(true);
                else _canAwaitLeftClick = true;
            }
        }

        private void SetClickInfo(Vector3 position, bool isLeft)
        {
            _currentClickInfo = new(isLeft, position);
            currentAwaiter?.Finish();
        }

        private void ProcessDrag(Vector3 startDrag, Vector3 finishDrag)
        {
            // Drag is opposed to mouse movement
            if (_pressedTimer > _clickDuration) OnDrag?.Invoke(startDrag - finishDrag);
        }
    }
}
