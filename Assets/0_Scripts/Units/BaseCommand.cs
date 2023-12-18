using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Rot
{
    public abstract class BaseCommand : ICommand
    {
        public enum AdditionalInput { None, Position, Target, Tile}

        public BaseCommand(string name)
        {
            Name = name;
            IsFinished = false;
        }

        public bool IsFinished { get; private set; }
        public abstract Task<int> Execute(int availableSpeed);
        public virtual void Finish()
        {
            IsFinished = true;
            Debug.Log($"Finishing command {Name}");
        }

        public Sprite Icon { get; protected set; }
        public string Name { get;  protected set; }
        public AdditionalInput ExtraInput { get; protected set; }
        public virtual void SetTarget(IDamagable target) { }
        public virtual void SetPath(Path path) { }
        public virtual void SetTile(Tile targetTile) { }
    }

    internal class AttackCommand : BaseCommand
    {
        private Skill _skill;
        private IDamagable _target;

        public AttackCommand(Skill skill) : base("Attack")
        {
            ExtraInput = AdditionalInput.Target;
            _skill = skill;
        }

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

        public Path Path { get; private set; }
        public int MaxSpeed { get; private set; }

        public MoveCommand(UnitView view, int maxSpeed) : base("Move")
        {
            _view = view;
            MaxSpeed = maxSpeed;
            ExtraInput = AdditionalInput.Position;
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            int remainingSpeed = availableSpeed;

            if (Path == null || Path.IsFinished) Finish();
            else
            {
                if (Path.NextStepCost <= remainingSpeed)
                {
                    remainingSpeed -= Path.NextStepCost;
                    await _view.MoveTo(Path.GetNextStep());
                    if (Path.IsFinished) Finish();
                }
                else if (Path.NextStepCost > MaxSpeed)
                {
                    Debug.Log("Cannot follow path");
                    Finish();
                }
                else remainingSpeed = 0;
            }
            return remainingSpeed;
        }
        public override void SetPath(Path path) => Path = path;
    }

    internal class DefendCommand : BaseCommand
    {
        private Func<Task> _requestHeal;
        public DefendCommand(Func<Task> requestHeal) : base("Defence") => 
            _requestHeal = requestHeal;

        public override async Task<int> Execute(int availableSpeed)
        {
            //await _requestHeal?.Invoke();
            return 0;
        }
    }

    internal class InfluenceCommand : BaseCommand
    {
        private IReceivingInfluence _target;
        private int _influence;

        public InfluenceCommand(int influence) : base("Influence")
        {
            _influence = influence;
            ExtraInput = AdditionalInput.Tile;
        }

        public override async Task<int> Execute(int availableSpeed)
        {
            _target?.ReceiveExternalInfluence(_influence);
            Finish();
            return 0;
        }

        public override void SetTile(Tile targetTile) => _target = targetTile;
    }

    internal class BuildCommand : BaseCommand
    {
        private IBuildable _build;
        private int _counter;
        private bool _buildComplete = false;

        public BuildCommand() : base("Build")
        {
            ExtraInput = AdditionalInput.Tile;
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

        public override void SetTile(Tile targetTile)
        {
            _build = targetTile;
            _counter = _build.RequestBuildRequirements();
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
