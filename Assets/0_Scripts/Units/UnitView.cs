using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public class UnitView : MonoBehaviour
    {
        public Func<Vector2Int, Vector3> ToWorldCoordinates;
        public async Task MoveTo(Vector2Int newPosition)
        {
            transform.position = ToWorldCoordinates(newPosition);
        }
    }
}
