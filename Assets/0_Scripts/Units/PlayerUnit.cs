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

        public PlayerUnit(UnitView view, Vector2Int initialPosition, int maxDefence, int speed, string name)
            : base(view, initialPosition, true, maxDefence, speed)
        {
            Name = name;
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
