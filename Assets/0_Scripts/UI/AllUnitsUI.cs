using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class AllUnitsUI : MonoBehaviour
    {
        [SerializeField] private Button _unitButton;

        private List<PlayerUnit> _displayedUnits;

        public Action<PlayerUnit> OnUnitSelection;


        private void Awake()
        {
            _unitButton.onClick.AddListener(OnUnitClick);
        }
        public void DisplayUnits(List<PlayerUnit> units)
        {
            _displayedUnits = new(units);
        }


        private void OnUnitClick()
        {
            OnUnitSelection?.Invoke(_displayedUnits[0]);
        }
        private void OnDestroy()
        {
            _unitButton.onClick.RemoveAllListeners();
        }
    }
}
