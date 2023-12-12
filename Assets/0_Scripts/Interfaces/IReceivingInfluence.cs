using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IReceivingInfluence
    {
        int Vitality { get; }
        bool IsAlive { get; }
        void ReceiveExternalInfluence(int externalInfluence);
    }
}
