using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Compilation;
using UnityEngine;

namespace Rot
{
    public abstract class BaseUnit : IReceivingInfluence, IDamagable
    {
        private int _vitality;
        private int _minVitality;
        private int _maxVitality;

        private int _defence;
        private int _maxDefence;

        protected int _speed;
        protected int _level;

        protected Vector2Int modelPosition;
        protected UnitView unitView;
        protected List<Skill> skills;
        protected bool abortAct;

        protected ICommand currentCommand;

        public int Vitality
        {
            get => _vitality;
            protected set
            {
                _vitality = Mathf.Clamp(value, _minVitality, _maxVitality);
                if (_vitality == 0) Die();
            }
        }
        public int Defence
        {
            get => _defence;
            protected set => _defence = Mathf.Clamp(value, 0, _maxDefence);
        }

        public bool IsAlive => Vitality > 0;
        public bool HasTask => currentCommand != null;
        public bool FinishedActions { get; private set; }

        public string Name { get; set; }
        public Sprite Icon { get; protected set; }
        public Vector2Int ModelPosition => modelPosition;
        public Location InitialLocation { get; protected set; }

        internal Action OnTurnEnd { get; set; }
        internal Action OnDeath { get; set; }


        public BaseUnit(Vector2Int initialPosition, bool alive, int maxDefence, int speed)
        {
            modelPosition = initialPosition;
            
            if(alive)
            {
                _minVitality = 0;
                _maxVitality = BaseValues.BaseVitality;
                Vitality = _maxVitality;
            }
            else
            {
                _minVitality = -BaseValues.BaseVitality;
                _maxVitality = 0;
                Vitality = _minVitality;
            }

            _maxDefence = maxDefence;
            Defence = _maxDefence;
            _speed = speed;

            skills = new();
        }


        public void SetView(UnitView view)
        {
            unitView = view;
            view.SetInitialPosition(modelPosition);
            view.OnPositionChange = v => modelPosition = v;
        }
        public void SetLocation(Location location)
        {
            if (InitialLocation != null) InitialLocation.RemoveUnit(this);
            InitialLocation= location;
            InitialLocation.AddUnit(this);
        }
        public abstract void ReceiveExternalInfluence(int externalInfluence);
        public abstract void ReceiveDamage(int damage);
        public void InitAction() => FinishedActions = false;
        public async void Act(bool auto = false)
        {
            abortAct = false;
            bool _needInput = auto ? !HasTask : true;

            int remainingSpeed = _speed;

            Debug.Log($"Starting act of {Name}");

            while(remainingSpeed > 0)
            {
                if (_needInput) currentCommand = await RequestInput();
                if (abortAct)
                {
                    OnTurnEnd?.Invoke();
                    return;
                }

                remainingSpeed = await currentCommand.Execute(remainingSpeed);
                CheckCurrentCommand(ref _needInput);
            }
            FinishedActions = true;
            OnTurnEnd?.Invoke();
        }

        protected async Task<ICommand> RequestInput()
        {
            List<ICommand> availableCommands = new();

            availableCommands.Add(new MoveCommand(unitView, _speed));

            foreach (var skill in skills) availableCommands.Add(new AttackCommand(skill));

            availableCommands.AddRange(InitiateSpecificCommands());

            availableCommands.Add(new InfluenceCommand(Vitality));

            availableCommands.Add(new DefendCommand(RequestHeal));

            if (HasTask && currentCommand is MoveCommand mc) unitView.DrawPath(mc.Path.PathCoordinates);
            var res = await SelectCommand(availableCommands);
            unitView.DrawPath(null);
            return res;
        }
        protected abstract List<ICommand> InitiateSpecificCommands();
        protected abstract Task<ICommand> SelectCommand(List<ICommand> availableCommands);
        protected void TryChangeDefence(ref int change)
        {
            var targetDefence = Defence + change;
            Defence += change;
            change = targetDefence - Defence;
        }
        protected void TryChangeVitality(ref int change)
        {
            var targetVitality = Vitality + change;
            Vitality += change;
            change = targetVitality - Vitality;
        }
        protected void IncreaseLevel()
        {
            if(_level < 3) _level++;
        }


        private void CheckCurrentCommand(ref bool needInput)
        {
            if (HasTask)
                if (currentCommand.IsFinished)
                {
                    currentCommand = null;
                    needInput = true;
                }
                else needInput = false;
            else needInput = true;
        }
        private async Task RequestHeal() => ReceiveDamage(-BaseValues.BaseVitality / 2);
        private void Die()
        {
            InitialLocation.RemoveUnit(this);
            unitView.transform.SetParent(null);
            GameObject.Destroy(unitView.gameObject);
            OnDeath?.Invoke();
        }
    }
}
