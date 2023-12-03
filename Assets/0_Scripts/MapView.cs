using Rot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    internal class MapView : MonoBehaviour
    {
        [SerializeField] Transform tilePrefab;
        [SerializeField] Camera _camera;

        private Vector2[] _baseVectors;
        private Vector2[] _inversedBaseVectors;

        private Dictionary<Vector2Int, TileView> _drawnTiles;

        private Vector3 _currentClick;
        private Plane _plane = new Plane(Vector3.up, 0);

        private void Start()
        {
            _baseVectors = new Vector2[2] { new Vector2(0.866f, 0), new Vector2(-0.433f, 0.75f)};
            SetInverseBaseVectors();
        }

        private void Update()
        {
            float distance;

            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if(_plane.Raycast(ray, out distance)) _currentClick = ray.GetPoint(distance);
                Vector2Int modelCoordinates = WorldToMapCoordinates(_currentClick);
                Debug.Log($"Click at {_currentClick}, MapCoordinates: {modelCoordinates}");
                if (_drawnTiles.ContainsKey(modelCoordinates))
                {
                    _drawnTiles[modelCoordinates]?.OnClick();
                }
            }
        }


        internal void DrawMap(List<Tile> tiles)
        {
            Clear();
            _drawnTiles = new();

            foreach (Tile tile in tiles)
            {
                var t = Instantiate(tilePrefab, transform);
                t.position = MapToWorldCoordinates(tile.ModelPosition);
                _drawnTiles.Add(tile.ModelPosition, t.GetComponent<TileView>());
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

        private void SetInverseBaseVectors()
        {
            float v11 = _baseVectors[0].x;
            float v12 = _baseVectors[0].y;
            float v21 = _baseVectors[1].x;
            float v22 = _baseVectors[1].y;

            float det = v11 * v22 - v12 * v21;

            float w11 = v22 / det;
            float w12 = -v12 / det;
            float w21 = -v21 / det;
            float w22 = v11 / det;

            _inversedBaseVectors = new Vector2[2] { new Vector2(w11, w12), new Vector2(w21, w22)};
        }

        private Vector3 MapToWorldCoordinates(Vector2Int mapCoordinates)
        {
            Vector2 Vc = HexToWorld(mapCoordinates);
            return new Vector3(Vc.x, 0, Vc.y);
        }

        private Vector2Int WorldToMapCoordinates(Vector3 worldCoordinates)
        {
            Vector2 Vc = new Vector2(worldCoordinates.x, worldCoordinates.z);
            return WorldToHex(Vc);
        }

        private Vector2 HexToWorld(Vector2Int mapCoordinates)
        {
            float newX = _baseVectors[0].x * mapCoordinates.x + _baseVectors[1].x * mapCoordinates.y;
            float newY = _baseVectors[0].y * mapCoordinates.x + _baseVectors[1].y * mapCoordinates.y;

            return new Vector2(newX, newY);
        }

        private Vector2Int WorldToHex(Vector2 worldCoordinates)
        {
            float newX = _inversedBaseVectors[0].x * worldCoordinates.x + _inversedBaseVectors[1].x * worldCoordinates.y;
            float newY = _inversedBaseVectors[0].y * worldCoordinates.x + _inversedBaseVectors[1].y * worldCoordinates.y;
            float newZ = 0 - newX - newY;

            int x = Mathf.RoundToInt(newX);
            int y = Mathf.RoundToInt(newY);
            int z = Mathf.RoundToInt(newZ);

            float diffX = Mathf.Abs(newX - x);
            float diffY = Mathf.Abs(newY - y);
            float diffZ = Mathf.Abs(newZ - z);


            if (diffX > diffY && diffX > diffZ) x = 0 - y - z;
            else if(diffY > diffZ) y = 0 - z - x;

            return new Vector2Int(x, y);
        }
    }
}
