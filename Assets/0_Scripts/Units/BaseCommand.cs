using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Rot
{
    public abstract class BaseCommand : ICommand
    {
        public enum AdditionalInput { None, Position, Target}

        public BaseCommand(string name) => Name = name;

        public bool IsFinished { get; private set; }
        public abstract Task<int> Execute(int availableSpeed);
        public virtual void Finish() => IsFinished = true;

        public Sprite Icon { get; protected set; }
        public string Name { get;  protected set; }
        public AdditionalInput ExtraInput { get; protected set; }
        public virtual void SetTarget(IDamagable target) { }
        public virtual void SetTargetPosition(Vector2Int position) { }
    }

    internal class AttackCommand : BaseCommand
    {
        private Skill _skill;
        private IDamagable _target;

        public AttackCommand(Skill skill) : base("Attack") => _skill = skill;

        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;

            if (_target != null && _skill.Cost <= availableSpeed)
            {
                _target.ReceiveDamage(_skill.Damage);
                remainingSpeed = 0;
            }

            Finish();
            return remainingSpeed;
        }
        public override void SetTarget(IDamagable target) => _target = target;
    }

    internal class MoveCommand : BaseCommand
    {
        private UnitView _view;
        private Vector2Int _currentPosition;
        private Stack<IPathInfo> _path;

        public MoveCommand(UnitView view, Vector2Int _currentposition) : base("Move")
        {
            _view = view;
            _currentPosition = _currentposition;
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;

            if (_path == null) Finish();
            else
            {
                if(_path.Peek().Cost <= remainingSpeed)
                {
                    var nextStep = _path.Pop();
                    await _view.MoveTo(nextStep.ModelPosition);
                    remainingSpeed -= nextStep.Cost;
                }

                if(_path.Count == 0) Finish();
            }
            return remainingSpeed;
        }
        public override void SetTargetPosition(Vector2Int position) =>
            _path = PathFinder.GetPath(_currentPosition, position);
    }

    internal class DefendCommand : BaseCommand
    {
        private Func<Task> _requestHeal;
        public DefendCommand(Func<Task> requestHeal) : base("Defence") => 
            _requestHeal = requestHeal;

        public override async Task<int> Execute(int availableSpeed)
        {
            await _requestHeal?.Invoke();
            Finish();
            return 0;
        }
    }

    internal class InfluenceCommand : BaseCommand
    {
        private IReceivingInfluence _target;
        private int _influence;

        public InfluenceCommand(IReceivingInfluence target, int influence) : base("Influence")
        {
            _target = target;
            _influence = influence;
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            _target?.ReceiveExternalInfluence(_influence);
            Finish();
            return 0;
        }
    }

    internal class BuildCommand : BaseCommand
    {
        private IBuildable _build;
        private int _counter;
        private bool _buildComplete = false;

        public BuildCommand(IBuildable build) : base("Build")
        {
            _build = build;
            _counter = _build.RequestBuildRequirements();
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;
            _counter -= remainingSpeed;
            if(_counter <= 0)
            {
                _buildComplete = true;
                Finish();
            }
            return 0;
        }

        public override void Finish()
        {
            _build.StopBuild(_buildComplete);
            base.Finish();
        }
    }

    internal class UpgradeCommand : BaseCommand
    {
        private int _counter;
        private Func<Task> _requestUpgrade;

        public UpgradeCommand(Func<Task> requestUpgrade) : base("Upgrade")
        {
            _requestUpgrade = requestUpgrade;
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;
            _counter -= remainingSpeed;
            if (_counter <= 0)
            {
                await _requestUpgrade();
                Finish();
            }
            return 0;
        }
    }
}
