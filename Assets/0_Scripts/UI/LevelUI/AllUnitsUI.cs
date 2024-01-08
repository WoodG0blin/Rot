using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class AllUnitsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _unitButtonPrefab;

        public Action<PlayerUnit> OnUnitSelection;


        public void DisplayUnits(List<PlayerUnit> units)
        {
            Clear();

            foreach(var unit in units)
            {
                var b = Instantiate(_unitButtonPrefab, transform).GetComponent<UnitPanelUI>();
                b.SetUnit(unit.Name, unit.Icon);
                b.OnClick = () => OnUnitSelection?.Invoke(unit);
            }
        }


        private void Clear()
        {
            int children = transform.childCount;
            if (children == 0) return;
            for(int i = children; i >0; i--)
            {
                var c = transform.GetChild(i-1);
                c.SetParent(null);
                GameObject.Destroy(c.gameObject);
            }
        }
    }
}
