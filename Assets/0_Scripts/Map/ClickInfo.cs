using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class ClickInfo
    {
        public bool IsLeftClick { get; private set; }
        public Vector3 ScreenCoordinates { get; private set; }
        public Vector3 WorldCoordinates { get; set; }
        public Vector2Int MapCoordinates { get; set; }

        public ClickInfo(bool isLeft, Vector3 screen)
        {
            IsLeftClick = isLeft;
            ScreenCoordinates = screen;
        }
    }
}
