using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public abstract class BaseLocation : IBuildable, IDamagable, IReceivingInfluence
    {
        private int _vitality;
        private int _maxVitality;
        private int _minVitality;

        private int _defence;
        private int _maxDefence;

        protected int counter;
        protected List<BaseUnit> unitList;

        public int Vitality
        {
            get => _vitality;
            protected set
            {
                _vitality = Mathf.Clamp(value, _minVitality, _maxVitality);
                if (_vitality == 0) Destroy();
            }
        }
        public int Defence
        {
            get => _defence;
            protected set => _defence = Mathf.Clamp(value, 0, _maxDefence);
        }

        public Vector2Int ModelPosition { get; protected set; }
        public string Name {get; set;}
        public int Level { get; protected set; }


        internal Action OnBuildComplete;
        internal Action<BaseUnit> OnNewUnit;
        internal Action OnDestruction;


        protected BaseLocation(Vector2Int position, bool alive)
        {
            ModelPosition = position;
            Level = 0;
            unitList = new();

            _maxVitality = alive ? BaseValues.BaseVitality : 0;
            _minVitality = _maxVitality - BaseValues.BaseVitality;

            _maxDefence = BaseValues.BaseVitality;

            Vitality = alive ? _maxVitality : _minVitality;
            Defence = _maxDefence;

            counter = 0;
        }


        public void TryGetInfluence(ref int influence)
        {
            TryChangeDefence(ref influence);
            TryChangeVitality(ref influence);
        }

        public abstract void Act();
        public abstract void ReceiveDamage(int damage);


        internal virtual void AddUnit(BaseUnit unit)
        {
            unitList.Add(unit);
        }
        internal virtual void RemoveUnit(BaseUnit unit)
        {
            if (unitList.Contains(unit)) unitList.Remove(unit);
        }


        bool IBuildable.IsBuildAvailable => Level < 3;
        virtual public int RequestBuildRequirements() => 0;
        void IBuildable.StopBuild(bool finished)
        {
            if (finished) Level++;
            OnBuildComplete?.Invoke();
        }


        protected virtual void TryChangeDefence(ref int influence)
        {
            var targetDefence = Defence + influence;
            Defence += influence;
            influence = targetDefence - Defence;
        }
        protected void TryChangeVitality(ref int influence)
        {
            var targetVitality = Vitality + influence;
            Vitality += influence;
            influence = targetVitality - Vitality;
        }
        protected void Destroy()
        {
            OnDestruction?.Invoke();
            OnNewUnit = null;
            OnBuildComplete = null;
            OnDestruction = null;
            unitList = null;
        }
    }

    public class PlayerLocation : BaseLocation
    {
        internal int UnitsLimit { get; private set; }

        internal PlayerLocation(Vector2Int position) : base(position, true)
        {
            UnitsLimit = 3;
        }


        override public void Act()
        {
            if(unitList.Count < UnitsLimit)
            {
                counter++;
                if(counter == 5)
                {
                    var unit = new PlayerUnit(ModelPosition, BaseValues.BaseVitality, 3);
                    unit.SetLocation(this);
                    OnNewUnit?.Invoke(unit);
                    counter = 0;
                }
            }
        }

        public override void ReceiveDamage(int damage)
        {
            int infl = -damage;
            TryGetInfluence(ref infl);
        }

        override public int RequestBuildRequirements() => (Level + 1) * 5;
    }
}
