using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public interface IBuildable
    {
        bool IsBuildAvailable { get; }
        int RequestBuildRequirements();
        void StopBuild(bool finished);
    }
}
