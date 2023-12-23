using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    internal class EnemyManager
    {
        private List<BaseUnit> _units;
        private List<EnemyLocation> _locations;
        private List<Tile> _tiles;

        public Action<BaseUnit> RegisterNewUnit;
        public Action<EnemyLocation> OnNewLocation;


        internal EnemyManager()
        {
            _units = new();
            _locations= new();
            _tiles= new();
        }


        public void Act()
        {
            foreach(var l in _locations) l.Act();
            foreach(var u in _units) u.Act();
        }


        internal void AddNewTile(Tile tile)
        {
            if(!_tiles.Contains(tile))
            {
                _tiles.Add(tile);
                var l = new EnemyLocation(tile.ModelPosition, tile.Location);
                l.OnNewUnit = u => AddUnit(u as EnemyUnit);
                _locations.Add(l);
                OnNewLocation(l);
            }
        }
        internal void RemoveTile(Tile tile)
        {
            if(_tiles.Contains(tile))
            {
                _tiles.Remove(tile);
                tile.RemoveLocation();
                if(tile.Location is EnemyLocation l ) _locations.Remove(l);
            }
        }


        private void AddUnit(EnemyUnit unit)
        {
            unit.Name = $"EnemyUnit{_units.Count}";
            unit.OnDeath = () => _units.Remove(unit);

            _units.Add(unit);
            RegisterNewUnit?.Invoke(unit);
        }
    }
}
