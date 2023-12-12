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


        internal Action OnDeath { get; set; }
        internal Action OnTurnEnd { get; set; }
        internal Func<Vector2Int, Tile> GetTile { get; set; }


        public BaseUnit(UnitView view, bool alive, int maxDefence, int speed)
        {
            unitView= view;
            
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
        }


        public abstract void ReceiveExternalInfluence(int externalInfluence);
        public abstract void ReceiveDamage(int damage);
        public async Task Act(bool auto = false)
        {
            bool _needInput = auto ? currentCommand == null : true;

            int remainingSpeed = _speed;

            while(remainingSpeed > 0)
            {
                if (_needInput) currentCommand = await RequestInput();
                if (currentCommand == null) return;

                remainingSpeed -= await currentCommand.Execute(remainingSpeed);
                CheckCurrentCommand(ref _needInput);
            }
        }


        protected async Task<ICommand> RequestInput()
        {
            List<ICommand> availableCommands = new();

            availableCommands.Add(new MoveCommand(unitView, RequestPath));

            foreach (var skill in skills) availableCommands.Add(new AttackCommand(skill, RequestTarget));

            availableCommands.AddRange(InitiateSpecificCommands());

            availableCommands.Add(new InfluenceCommand(GetTile(modelPosition), Vitality));

            availableCommands.Add(new DefendCommand(RequestHeal));

            return await SelectCommand(availableCommands);
        }
        protected abstract List<ICommand> InitiateSpecificCommands();
        protected abstract Task<ICommand> SelectCommand(List<ICommand> availableCommands);
        protected abstract Task<Stack<IPathInfo>> RequestPath();
        protected abstract Task<IDamagable> RequestTarget();
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
            if (currentCommand != null && currentCommand.IsFinished)
            {
                currentCommand = null;
                needInput = true;
            }
        }
        private async Task RequestHeal() => ReceiveDamage(-BaseValues.BaseVitality / 2);
        private void Die() => OnDeath?.Invoke();
    }
}
