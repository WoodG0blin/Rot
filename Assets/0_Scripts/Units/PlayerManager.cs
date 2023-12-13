using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Rot
{
    internal class PlayerManager
    {
        private UIManager _UImanager;

        private GameObject _unitPrefab;
        private MapModel _mapModel;

        private List<PlayerUnit> _allUnits;
        private List<PlayerUnit> _killedUnits;
        
        private PlayerUnit _currentAct;
        private TurnController _turnController;
        private bool _canFinishTurn;
        private bool _autoMode;

        public Action OnTurnEnd;


        public PlayerManager(UIManager UImanager, GameObject unitPrefab, MapModel model)
        {
            _UImanager = UImanager;
            _UImanager.OnUnitSelection = ChangeSelectedUnit;

            _turnController = new();
            _turnController.OnReadyToFinishTurn = () => _canFinishTurn = true;

            _unitPrefab = unitPrefab;
            _mapModel = model;
            _allUnits = new();
            _killedUnits = new();
        }

        public void Act()
        {
            ClearKilled();
            _canFinishTurn = false;
            _autoMode = false;

            _turnController.Setup(_allUnits);

            SetNextTurn();
        }


        public void AddUnit(Vector2Int position)
        {
            UnitView view = GameObject.Instantiate(_unitPrefab).GetComponent<UnitView>();
            PlayerUnit newUnit = new(view, position, BaseValues.BaseVitality, 2);
            newUnit.RequestCommand = async c => await _UImanager.GetCommand(newUnit, c);
            newUnit.GetTile = p => _mapModel[p.x, p.y];
            newUnit.OnDeath = () => RemoveUnit(newUnit);
            _allUnits.Add(newUnit);
        }

        private async void SetNextTurn()
        {
            if (_canFinishTurn)
            {
                _autoMode = await _UImanager.FinishTurnClick();
                _canFinishTurn = false;
            }

            _currentAct = _turnController.GetNextTurn();

            if (_currentAct != null) _currentAct.Act(_autoMode);
            else
            {
                await _UImanager.FinishTurnClick();
                OnTurnEnd?.Invoke();
            }
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
        private void ChangeSelectedUnit(PlayerUnit unit) => _turnController.SetNext(unit);
    }

    internal class TurnController
    {
        private List<PlayerUnit> _requiredTurn;
        private List<PlayerUnit> _autoTurn;

        private PlayerUnit _currentTurn;

        public Action OnReadyToFinishTurn;
        public Action OnFinishTurn;

        internal TurnController()
        {
            _requiredTurn = new();
            _autoTurn = new();
        }

        public void Setup(List<PlayerUnit> allUnits)
        {
            _requiredTurn.Clear();
            _autoTurn.Clear();

            foreach (var unit in allUnits)
            {
                unit.InitAction();
                if (unit.HasTask) _autoTurn.Add(unit);
                else _requiredTurn.Add(unit);
            }
        }

        public PlayerUnit GetNextTurn()
        {
            if (_currentTurn != null && !_currentTurn.FinishedActions) _requiredTurn.Add(_currentTurn);

            _currentTurn = null;

            if (_requiredTurn.Count > 0)
            {
                _currentTurn = _requiredTurn[0];
                _requiredTurn.RemoveAt(0);
                if (_requiredTurn.Count == 0) OnReadyToFinishTurn?.Invoke();
            }
            else if (_autoTurn.Count > 0)
            {
                _currentTurn = _autoTurn[0];
                _autoTurn.RemoveAt(0);
            }

            return _currentTurn;
        }

        public void SetNext(PlayerUnit unit)
        {
            if (_requiredTurn.Contains(unit)) _requiredTurn.Remove(unit);
            if (_autoTurn.Contains(unit)) _autoTurn.Remove(unit);

            _requiredTurn.Insert(0, unit);
        }
    }
}
