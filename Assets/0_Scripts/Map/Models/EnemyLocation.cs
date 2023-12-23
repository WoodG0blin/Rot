using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    public class EnemyLocation : BaseLocation
    {
        internal EnemyLocation(Vector2Int position, BaseLocation initialLocation) : base(position, false)
        {
            if (initialLocation != null) Level = initialLocation.Level;
        }

        public override void Act()
        {
            counter++;
            if (counter == 5)
            {
                var unit = new EnemyUnit(ModelPosition, BaseValues.BaseVitality, 3);
                unit.SetLocation(this);
                OnNewUnit?.Invoke(unit);
                counter = 0;
            }
        }
        public override void ReceiveDamage(int damage) => 
            TryGetInfluence(ref damage);

        protected override void TryChangeDefence(ref int influence)
        {
            int infl = -influence;
            base.TryChangeDefence(ref infl);
        }
    }
}
