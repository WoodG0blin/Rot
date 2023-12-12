using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public class PlayerUnit : BaseUnit
    {
        public PlayerUnit(UnitView view, Vector2Int initialPosition, int maxDefence, int speed)
            : base(view, initialPosition, true, maxDefence, speed)
        {
        }

        public override void ReceiveDamage(int damage)
        {
            ReceiveExternalInfluence(-damage);
        }

        public override void ReceiveExternalInfluence(int externalInfluence)
        {
            int value = externalInfluence;
            TryChangeDefence(ref value);
            TryChangeVitality(ref value);
        }


        protected override List<ICommand> InitiateSpecificCommands()
        {
            List<ICommand> availableCommands = new();

            if(GetTile(modelPosition).Buildable.IsBuildAvailable)
                availableCommands.Add(new BuildCommand(GetTile(modelPosition).Buildable));

            return availableCommands;
        }
        protected override async Task<ICommand> SelectCommand(List<ICommand> availableCommands)
        {
            return availableCommands[0];
        }
        protected override async Task<Stack<IPathInfo>> RequestPath()
        {
            await Task.Delay(500);
            return PathFinder.GetPath(modelPosition, new Vector2Int(0, 0));
        }
        protected override async Task<IDamagable> RequestTarget()
        {
            await Task.Delay(500);
            return null;
        }
    }
}
