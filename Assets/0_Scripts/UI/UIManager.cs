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
        [SerializeField] private ChoicePanelUI _choiceUI;
        [SerializeField] private Button _nextTurnButton;


        internal Func<Action, Task<ClickInfo>> RequestClickInfo;
        internal Func<Vector2Int, List<IDamagable>> GetDamagablesAt;
        internal Func<Vector2Int, Tile> GetTile;

        internal Action<PlayerUnit> OnUnitSelection;

        internal void Init() { }

        public void SetUnitsList(List<PlayerUnit> unitsList) => _allUnitsUI.DisplayUnits(unitsList);
        public async Task<ICommand> GetCommand(PlayerUnit unit, List<ICommand> availableCommands)
        {
            _selectedUnitUI.SetSelectedUnit(unit);
            return await _selectedUnitUI.ChooseCommandFrom(availableCommands);
        }
        public async Task<bool> FinishTurnClick()
        {
            _nextTurnButton.image.color = Color.red;
            bool result = await this;
            currentAwaiter = null;
            _nextTurnButton.image.color = Color.grey;

            return result;
        }


        private void Awake()
        {
            _nextTurnButton.onClick.AddListener(OnFinishTurnClick);
            _allUnitsUI.OnUnitSelection = OnUnitSelectionRequest;
            _selectedUnitUI.RequestPathFrom = RequestPathFrom;
            _selectedUnitUI.RequestTarget = RequestTarget;
            _selectedUnitUI.RequestTile = RequestTile;
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
        private async Task<Path> RequestPathFrom(Vector2Int startPosition, Action cancellation, int maxSpeed = 1000)
        {
            var result = await RequestClickInfo(cancellation);
            if (result != null && !result.IsLeftClick) return PathFinder.GetPath(startPosition, result.MapCoordinates, maxSpeed);
            return null;
        }
        private async Task<IDamagable> RequestTarget(Action cancellation)
        {
            var result = await RequestClickInfo(cancellation);
            if (result != null && !result.IsLeftClick)
                return await _choiceUI.GetChoiceAt(GetDamagablesAt(result.MapCoordinates), result.ScreenCoordinates, cancellation);
            return null;
        }
        private Tile RequestTile(Vector2Int position) => GetTile(position);
        private void OnDestroy()
        {
            _nextTurnButton.onClick.RemoveAllListeners();
        }
    }
}
