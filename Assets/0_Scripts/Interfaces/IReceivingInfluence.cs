using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IReceivingInfluence
    {
        //int Vitality { get; }
        void TryGetInfluence(ref int externalInfluence);
    }
}
