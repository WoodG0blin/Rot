using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Rot
{
    internal interface IPathFinderTile : IPathInfo
    {
        List<IPathFinderTile> AdjoiningTiles { get; }

        int PathCostEstimate { get; set; }
        int CostSoFar { get; set; }
        bool NewPath { get; set; }

        IPathFinderTile Previous { get; set; }
    }

    internal static class PathFinder
    {
        private static MapModel _currentModel;

        internal static void Init(MapModel mapModel)
        {
            Debug.Log($"Current model is not null: {mapModel != null}");
            _currentModel = mapModel;
        }

        internal static Path GetPath(Vector2Int startPosition, Vector2Int finishPosition, int maxCost = 1000)
        {
            List<IPathFinderTile> _tilesToCheck = new();
            List<IPathFinderTile> _visited = new();

            IPathFinderTile startTile = _currentModel[startPosition.x, startPosition.y];
            IPathFinderTile finishTile = _currentModel[finishPosition.x, finishPosition.y];


            _tilesToCheck.Add(startTile);

            while (_tilesToCheck.Count > 0)
            {
                IPathFinderTile current = _tilesToCheck.OrderBy(t => t.PathCostEstimate).First();
                _visited.Add(current);
                _tilesToCheck.Remove(current);

                if (current.ModelPosition == finishTile.ModelPosition) break;

                foreach (IPathFinderTile newTile in current.AdjoiningTiles)
                {
                    if (_visited.Contains(newTile) || newTile == null) continue;

                    if (!_tilesToCheck.Contains(newTile))
                    {
                        newTile.NewPath = true;
                        newTile.Previous = null;
                        _tilesToCheck.Add(newTile);
                    }

                    TrySetNewPath(current, newTile, finishTile, maxCost);
                }
            }

            return SetPath(startTile, finishTile, maxCost);
        }


        private static void TrySetNewPath(IPathFinderTile current, IPathFinderTile next, IPathFinderTile target, int maxCost)
        {
            int currentEstimate = current.CostSoFar + AdjustToLimit(next.Cost, maxCost) + GetEuristic(next, target);
            
            if(next.NewPath || currentEstimate < next.PathCostEstimate)
            {
                next.CostSoFar = current.CostSoFar + next.Cost;
                next.PathCostEstimate = currentEstimate;
                next.Previous = current;
                next.NewPath = false;
            }
        }

        private static int AdjustToLimit(int cost, int maxCost) =>
            cost < maxCost ? cost : 1000;

        private static Path SetPath(IPathFinderTile start, IPathFinderTile finish, int maxCost)
        {
            Stack<IPathInfo> rawPath = new();
            Path path = new();

            IPathFinderTile next = finish;
            while(next.ModelPosition != start.ModelPosition)
            {
                rawPath.Push(next);
                next = next.Previous;
            }

            List<IPathInfo> temp = new();
            while(rawPath.Count > 0)
            {
                var t = rawPath.Pop();
                if (t.Cost <= maxCost) path.AddStep(t);
                else break;
            }

            return path;
        }

        private static int GetEuristic(IPathFinderTile next, IPathFinderTile target) =>
            Mathf.RoundToInt(Vector2.SqrMagnitude(target.ModelPosition - next.ModelPosition));
    }

    public class Path
    {
        private List<IPathInfo> _path;
        private int _index;

        public Path()
        {
            _path = new();
            _index = 0;
        }

        public bool IsFinished => _index >= _path.Count;
        public int NextStepCost => _path[_index].Cost;
        public List<Vector2Int> PathCoordinates => _path.GetRange(_index, _path.Count-_index).Select(t => t.ModelPosition).ToList();
        public bool TryGetNextStep(out Vector2Int nextPosition)
        {
            if(_index < _path.Count)
            {
                nextPosition = _path[_index].ModelPosition;
                _index++;
                return true;
            }
            nextPosition = Vector2Int.zero;
            return false;
        }
        public Vector2Int GetNextStep()
        {
            Vector2Int nextPosition = Vector2Int.zero;
            if (_index < _path.Count)
            {
                nextPosition = _path[_index].ModelPosition;
                _index++;
            }
            return nextPosition;
        }

        public void AddStepinversed(IPathInfo path) => _path.Insert(0, path);
        public void AddStep(IPathInfo path) => _path.Add(path);
    }
}
