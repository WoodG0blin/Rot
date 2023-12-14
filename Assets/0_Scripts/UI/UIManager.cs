using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class UIManager : AwaitableView
    {
        [SerializeField] private AllUnitsUI _allUnitsUI;
        [SerializeField] private SelectedUnitUI _selectedUnitUI;
        [SerializeField] private Button _nextTurnButton;

        public Action<PlayerUnit> OnUnitSelection;


        public void SetUnitsList(List<PlayerUnit> unitsList) => _allUnitsUI.DisplayUnits(unitsList);
        public async Task<ICommand> GetCommand(PlayerUnit unit, List<ICommand> availableCommands)
        {
            _selectedUnitUI.SetSelectedUnit(unit);
            return await _selectedUnitUI.ChooseCommandFrom(availableCommands);
        }
        public async Task<bool> FinishTurnClick()
        {
            bool result = await this;
            currentAwaiter = null;
            return result;
        }


        private void Awake()
        {
            _nextTurnButton.onClick.AddListener(OnFinishTurnClick);
            _allUnitsUI.OnUnitSelection = OnUnitSelectionRequest;
        }
        private void OnFinishTurnClick()
        {
            _selectedUnitUI.AbortAwait();
            currentAwaiter?.Finish(true);
        }
        private void OnUnitSelectionRequest(PlayerUnit unit)
        {
            OnUnitSelection?.Invoke(unit);
            _selectedUnitUI.AbortAwait();
            currentAwaiter?.Finish(false);
        }
        private void OnDestroy()
        {
            _nextTurnButton.onClick.RemoveAllListeners();
        }
    }
}
