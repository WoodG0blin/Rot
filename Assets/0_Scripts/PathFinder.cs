using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rot
{
    internal interface IPathFinderTile
    {
        Vector2Int ModelPosition { get; }
        List<IPathFinderTile> AdjoiningTiles { get; }
        int Cost { get; }

        int PathCostEstimate { get; set; }
        int CostSoFar { get; set; }
        bool NewPath { get; set; }

        IPathFinderTile Previous { get; set; }
    }

    internal static class PathFinder
    {
        internal static Stack<Vector2Int> GetPath(IPathFinderTile startTile, IPathFinderTile finishTile)
        {
            List<IPathFinderTile> _tilesToCheck = new();
            List<IPathFinderTile> _visited = new();

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

        private static Stack<Vector2Int> SetPath(IPathFinderTile start, IPathFinderTile finish)
        {
            Stack<Vector2Int> path = new();
            IPathFinderTile next = finish;
            while(next.ModelPosition != start.ModelPosition)
            {
                path.Push(next.ModelPosition);
                next = next.Previous;
            }
            return path;
        }

        private static int GetEuristic(IPathFinderTile next, IPathFinderTile target) =>
            Mathf.RoundToInt(Vector2.SqrMagnitude(target.ModelPosition - next.ModelPosition));
    }
}
