using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    internal class Location
    {
        private int _vitality;
        private int _maxVitality;

        private int _defence;
        private int _maxDefence;

        internal int Vitality
        {
            get => IsAlive? _vitality : 0;
            private set
            {
                _vitality = Mathf.Clamp(value, 0, _maxVitality);
                if(_vitality == 0) IsAlive = false;
            }
        }
        internal int Defence
        {
            get => _defence;
            private set => _defence = Mathf.Clamp(value, 0, _maxDefence);
        }

        internal int UnitsLimit { get; private set; }
        internal bool IsAlive { get; private set; }
        
        internal Location(int maxVitality, int maxDefence)
        {
            _maxVitality= maxVitality;
            _maxDefence= maxDefence;

            Vitality = _maxVitality;
            Defence = _maxDefence;

            UnitsLimit = 3;
            IsAlive = true;
        }


        internal void TryGetInfluence(ref int influence)
        {
            if (IsAlive)
            {
                TryChangeDefence(ref influence);
                TryChangeVitality(ref influence);
            }
        }

        private void TryChangeDefence(ref int change)
        {
            var targetDefence = Defence + change;
            Defence += change;
            change = targetDefence - Defence;
        }

        private void TryChangeVitality(ref int change)
        {
            var targetVitality = Vitality + change;
            Vitality += change;
            change = targetVitality - Vitality;
        }
    }
}
