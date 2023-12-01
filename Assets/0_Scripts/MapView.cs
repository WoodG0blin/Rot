using Rot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    internal class MapView : MonoBehaviour
    {
        [SerializeField] Transform tilePrefab;

        private const float _Xoffset = -0.5f;
        private const float _YoffsetCoeff = 1.059f;

        internal void DrawMap(List<Tile> tiles)
        {
            Clear();

            foreach (Tile tile in tiles)
            {
                var t = Instantiate(tilePrefab, transform);
                t.position = MapToWorldCoordinates(tile.ModelPosition);
            }
        }


        private void Clear()
        {
            for(int i = transform.childCount - 1; i >=0; i--)
            {
                var tr = transform.GetChild(i);
                tr.SetParent(null);
                GameObject.Destroy(tr.gameObject);
            }
        }

        private Vector3 MapToWorldCoordinates(Vector2Int mapCoordinates)
        {
            return new Vector3(mapCoordinates.x + _Xoffset * mapCoordinates.y, 0, mapCoordinates.y * _YoffsetCoeff);
        }
    }
}
