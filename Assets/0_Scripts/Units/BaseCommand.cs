using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Rot
{
    internal abstract class BaseCommand : ICommand
    {
        public bool IsFinished { get; private set; }
        public abstract Task<int> Execute(int availableSpeed);

        public virtual void Finish() => IsFinished = true;
    }

    internal class AttackCommand : BaseCommand
    {
        private Skill _skill;
        private Func<Task<IDamagable>> _requestTarget;
        private IDamagable _target;

        public AttackCommand(Skill skill, Func<Task<IDamagable>> requestTarget)
        {
            _skill = skill;
            _requestTarget = requestTarget;
        }
        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;

            _target = await _requestTarget();

            if (_target != null && _skill.Cost <= availableSpeed)
            {
                _target.ReceiveDamage(_skill.Damage);
                remainingSpeed = 0;
            }

            Finish();
            return remainingSpeed;
        }
    }

    internal class MoveCommand : BaseCommand
    {
        private UnitView _view;
        private Func<Task<Stack<IPathInfo>>> _requestPath;
        private Stack<IPathInfo> _path;
        public MoveCommand(UnitView view, Func<Task<Stack<IPathInfo>>> requestPath)
        {
            _view = view;
            _requestPath = requestPath;
        }
        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;

            _path ??= await _requestPath();

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
    }

    internal class DefendCommand : BaseCommand
    {
        private Func<Task> _requestHeal;
        public DefendCommand(Func<Task> requestHeal)
        {
            _requestHeal = requestHeal;
        }

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

        public InfluenceCommand(IReceivingInfluence target, int influence)
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

        public BuildCommand(IBuildable build)
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

        public UpgradeCommand(Func<Task> requestUpgrade)
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
