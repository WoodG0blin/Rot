using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    internal class PlayerManager
    {
        private SelectedUnitUI _selectedUnitUI;
        private AllUnitsUI _allUnitsUI;
        private GameObject _unitPrefab;
        private MapModel _mapModel;

        private List<PlayerUnit> _allUnits;
        private List<PlayerUnit> _killedUnits;
        
        private PlayerUnit _currentAct;
        private bool _canFinishTurn;
        public PlayerManager(SelectedUnitUI selectedUnitUI, AllUnitsUI allUnitsUI, GameObject unitPrefab, MapModel model)
        {
            _selectedUnitUI = selectedUnitUI;

            _allUnitsUI = allUnitsUI;
            _allUnitsUI.OnUnitSelection = ChangeSelectedUnit;

            _unitPrefab = unitPrefab;
            _mapModel = model;
            _allUnits = new();
            _killedUnits = new();
        }

        public async Task Act()
        {
            ClearKilled();
            _canFinishTurn = false;

            foreach(var unit in _allUnits) unit.InitAction();

            List<PlayerUnit> requiredTurn = new(_allUnits.Where(u => !u.HasTask));

            if (requiredTurn != null && requiredTurn.Count > 0)
                foreach (var unit in requiredTurn) await StartUnitAct(unit);

            while(!_canFinishTurn)
                if(await _selectedUnitUI) await TryFinishTurn();        
        }


        public void AddUnit(Vector2Int position)
        {
            UnitView view = GameObject.Instantiate(_unitPrefab).GetComponent<UnitView>();
            PlayerUnit newUnit = new(view, position, BaseValues.BaseVitality, 2);
            newUnit.GetTile = p => _mapModel[p.x, p.y];
            newUnit.OnDeath = () => RemoveUnit(newUnit);
            _allUnits.Add(newUnit);
        }


        private void RemoveUnit(PlayerUnit unit)
        {
            if(!_killedUnits.Contains(unit)) _killedUnits.Add(unit);
        }
        private void ClearKilled()
        {
            foreach (var unit in _killedUnits)
                if (_allUnits.Contains(unit)) _allUnits.Remove(unit);
            _killedUnits.Clear();
        }
        private async Task StartUnitAct(PlayerUnit unit, bool auto = false)
        {
            _currentAct = unit;
            _selectedUnitUI.SetSelectedUnit(_currentAct);
            await _currentAct.Act(auto);
        }
        private async Task TryFinishTurn()
        {
            List<PlayerUnit> remainingUnits = new(_allUnits.Where(u => !u.FinishedActions));
            
            _canFinishTurn = remainingUnits == null || remainingUnits.Count == 0;
            if (_canFinishTurn) return;

            foreach(var unit in remainingUnits) await StartUnitAct(unit, true);

            remainingUnits = new(_allUnits.Where(u => !u.FinishedActions));
            _canFinishTurn = remainingUnits == null || remainingUnits.Count == 0;
        }
        private void ChangeSelectedUnit(PlayerUnit unit) { }
    }
}
