using Rot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Rot
{
    internal class MapView : MonoBehaviour
    {
        [SerializeField] Transform tilePrefab;
        [SerializeField] Transform _tileParent;
        [SerializeField] GameObject _unitPrefab;
        [SerializeField] GameObject _enemyUnitPrefab;
        [SerializeField] Transform _unitsParent;

        private Vector2[] _baseVectors;
        private Vector2[] _inversedBaseVectors;
        private Func<TileTypes, Material> _getMaterial;

        private Dictionary<Vector2Int, TileView> _drawnTiles;
        private bool _initiated = false;

        internal Action<Vector2Int> OnTileClick;

        internal void Init(Func<TileTypes, Material> getMaterial)
        {
            _getMaterial = getMaterial;
            Init();
        }
        internal void DrawMap(List<Tile> tiles)
        {
            if (!_initiated) Init();

            Clear();
            _drawnTiles = new();

            foreach (Tile tile in tiles)
            {
                var t = Instantiate(tilePrefab, _tileParent).GetComponent<TileView>();
                t.Init(MapToWorldCoordinates(tile.ModelPosition), _getMaterial?.Invoke(tile.Type));
                t.UpdateTile(tile.IsAlive, tile.Vitality, tile.Location);
                _drawnTiles.Add(tile.ModelPosition, t);
            }
        }

        internal void UpdateTiles(List<Tile> tiles)
        {
            foreach (var t in tiles)
                if (_drawnTiles.ContainsKey(t.ModelPosition))
                    _drawnTiles[t.ModelPosition].UpdateTile(t.IsAlive, t.Vitality, t.Location);
        }

        internal void ProcessClick(Vector3 position)
        {
            Vector2Int modelCoordinates = WorldToMapCoordinates(position);

            if (_drawnTiles.ContainsKey(modelCoordinates))
            {
                ClearDrawn();
                _drawnTiles[modelCoordinates]?.OnClick();
                OnTileClick?.Invoke(modelCoordinates);
            }
        }
        internal UnitView InitiateUnitView(bool player = true)
        {
            UnitView view = GameObject.Instantiate(player ? _unitPrefab : _enemyUnitPrefab, _unitsParent).GetComponent<UnitView>();
            view.ToWorldCoordinates = MapToWorldCoordinates;
            view.OnDrawPath = DrawPath;
            return view;
        }

        internal async void DrawAdjoining(List<Tile> tiles)
        {
            foreach(var t in tiles)
                _drawnTiles[t.ModelPosition].SetSelection(true);
        }

        internal void DrawPath(List<Vector2Int> path)
        {
            foreach(var t in _drawnTiles.Values) t.SetSelection(false);
            if (path == null) return;

            for(int i = 0; i < path.Count; i++)
                if (_drawnTiles.ContainsKey(path[i])) _drawnTiles[path[i]].SetSelection(true);
        }


        private void Init()
        {
            _baseVectors = new Vector2[2] { new Vector2(0.866f, 0), new Vector2(-0.433f, 0.75f)};
            SetInverseBaseVectors();
            _initiated = true;
        }

        private void Clear()
        {
            for(int i = _tileParent.childCount - 1; i >=0; i--)
            {
                var tr = _tileParent.GetChild(i);
                tr.SetParent(null);
                GameObject.Destroy(tr.gameObject);
            }
        }

        private void ClearDrawn()
        {
            foreach(var v in _drawnTiles.Values) v.SetSelection(false);
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

        internal Vector2Int WorldToMapCoordinates(Vector3 worldCoordinates)
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
