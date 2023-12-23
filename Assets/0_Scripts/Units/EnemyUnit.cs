using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public class EnemyUnit : BaseUnit
    {
        public EnemyUnit(Vector2Int initialPosition, int maxDefence, int speed) :
            base(initialPosition, false, maxDefence, speed) {}

        public override void ReceiveDamage(int damage) => TryGetInfluence(ref damage);

        protected override async Task<ICommand> SelectCommand(List<ICommand> availableCommands)
        {
            return availableCommands.Where(c => c is DefendCommand).First();
        }
        protected override void TryChangeDefence(ref int change)
        {
            int c = -change;
            base.TryChangeDefence(ref c);
        }
    }
}
