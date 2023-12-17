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

        Sprite Icon { get; }
        string Name { get; }
        BaseCommand.AdditionalInput ExtraInput { get; }

        void SetTarget(IDamagable target);
        void SetPath(Path path);
        void SetTile(Tile targetTile);
    }
}
