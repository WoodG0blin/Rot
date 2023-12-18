using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class Location : IBuildable
    {
        private int _counter;

        private int _vitality;
        private int _maxVitality;

        private int _defence;
        private int _maxDefence;

        private int _level;

        private List<BaseUnit> _unitList;

        internal int Vitality
        {
            get => IsAlive? _vitality : 0;
            private set
            {
                _vitality = Mathf.Clamp(value, 0, _maxVitality);
                if(_vitality == 0) IsAlive = false;
            }
        }
        internal int Defence
        {
            get => _defence;
            private set => _defence = Mathf.Clamp(value, 0, _maxDefence);
        }

        internal Vector2Int ModelPosition { get; private set; }
        internal int UnitsLimit { get; private set; }
        internal bool IsAlive { get; private set; }

        internal Action OnBuildComplete;
        internal Action<PlayerUnit> OnNewUnit;

        bool IBuildable.IsBuildAvailable => _level < 3;
        int IBuildable.RequestBuildRequirements()
        {
            return (_level + 1) * 5;
        }

        void IBuildable.StopBuild(bool finished)
        {
            if (finished) _level++;
            OnBuildComplete?.Invoke();
        }

        internal Location(Vector2Int position)
        {
            ModelPosition = position;
            _level = 0;
            _unitList = new();

            _maxVitality= BaseValues.BaseVitality;
            _maxDefence= BaseValues.BaseVitality;

            Vitality = _maxVitality;
            Defence = _maxDefence;

            UnitsLimit = 3;
            IsAlive = true;

            _counter = 0;
        }


        internal void Act()
        {
            if(_unitList.Count < UnitsLimit)
            {
                _counter++;
                if(_counter == 5)
                {
                    var unit = new PlayerUnit(ModelPosition, BaseValues.BaseVitality, 3);
                    unit.SetLocation(this);
                    OnNewUnit?.Invoke(unit);
                    _counter = 0;
                }
            }
        }
        internal void AddUnit(BaseUnit unit)
        {
            _unitList.Add(unit);
        }
        internal void RemoveUnit(BaseUnit unit)
        {
            if(_unitList.Contains(unit)) _unitList.Remove(unit);
        }

        internal void TryGetInfluence(ref int influence)
        {
            if (IsAlive)
            {
                TryChangeDefence(ref influence);
                TryChangeVitality(ref influence);
            }
        }

        private void TryChangeDefence(ref int change)
        {
            var targetDefence = Defence + change;
            Defence += change;
            change = targetDefence - Defence;
        }

        private void TryChangeVitality(ref int change)
        {
            var targetVitality = Vitality + change;
            Vitality += change;
            change = targetVitality - Vitality;
        }

    }
}
