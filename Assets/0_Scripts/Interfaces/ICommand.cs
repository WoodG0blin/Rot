using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    public interface ICommand
    {
        bool IsFinished { get; }
        Task<int> Execute(int availableSpeed);
        void Finish();
    }
}
