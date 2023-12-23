using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public class PlayerUnit : BaseUnit
    {
        public Func<List<ICommand>, Task<ICommand>> RequestCommand;

        public PlayerUnit(Vector2Int initialPosition, int maxDefence, int speed)
            : base(initialPosition, true, maxDefence, speed) {}

        public override void ReceiveDamage(int damage)
        {
            int influence = -damage;
            TryGetInfluence(ref influence);
        }


        protected override List<ICommand> InitiateSpecificCommands()
        {
            List<ICommand> availableCommands = new();

            availableCommands.Add(new BuildCommand());

            return availableCommands;
        }
        protected override async Task<ICommand> SelectCommand(List<ICommand> availableCommands)
        {
            var res = await RequestCommand?.Invoke(availableCommands);
            if (res == null)
            {
                abortAct = true;
                return currentCommand;
            }
            return res;
        }
    }
}
