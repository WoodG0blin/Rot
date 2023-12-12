using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        internal static Stack<IPathInfo> GetPath(Vector2Int startPosition, Vector2Int finishPosition)
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

                    TrySetNewPath(current, newTile, finishTile);
                }
            }

            return SetPath(startTile, finishTile);
        }


        private static void TrySetNewPath(IPathFinderTile current, IPathFinderTile next, IPathFinderTile target)
        {
            int currentEstimate = current.CostSoFar + next.Cost + GetEuristic(next, target);
            
            if(next.NewPath || currentEstimate < next.PathCostEstimate)
            {
                next.CostSoFar = current.CostSoFar + next.Cost;
                next.PathCostEstimate = currentEstimate;
                next.Previous = current;
                next.NewPath = false;
            }
        }

        private static Stack<IPathInfo> SetPath(IPathFinderTile start, IPathFinderTile finish)
        {
            Stack<IPathInfo> path = new();
            IPathFinderTile next = finish;
            while(next.ModelPosition != start.ModelPosition)
            {
                path.Push(next);
                next = next.Previous;
            }
            return path;
        }

        private static int GetEuristic(IPathFinderTile next, IPathFinderTile target) =>
            Mathf.RoundToInt(Vector2.SqrMagnitude(target.ModelPosition - next.ModelPosition));
    }
}
