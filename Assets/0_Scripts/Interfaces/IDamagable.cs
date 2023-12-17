using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IDamagable
    {
        string Name { get; }
        void ReceiveDamage(int damage);
    }
}
