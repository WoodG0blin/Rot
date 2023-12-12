using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IPathInfo
    {
        Vector2Int ModelPosition { get; }
        int Cost { get; }
    }
}
