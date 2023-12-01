using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class LevelManager : MonoBehaviour
    {
        void Start()
        {
            MapModel model = new(4);
            foreach (Tile tile in model.AllTiles) Debug.Log(tile.ModelPosition);
        }
    }
}
