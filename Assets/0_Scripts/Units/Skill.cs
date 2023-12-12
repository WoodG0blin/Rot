using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class Skill
    {
        public int Damage { get; private set; }
        public int Range { get; private set; }
        public int Cost { get; private set; }


        public void Attack(IReceivingInfluence target)
        {
            target.ReceiveExternalInfluence(Damage);
        }
    }
}
