using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapView _mapView;


        void Start()
        {
            MapModel model = new(4);
            _mapView.DrawMap(model.AllTiles);
        }
    }
}
