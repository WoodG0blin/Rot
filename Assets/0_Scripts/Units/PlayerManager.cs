using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rot
{
    internal class PlayerManager
    {
        private UIManager _UImanager;

        private List<PlayerUnit> _allUnits;
        private List<Location> _locations;

        private List<PlayerUnit> _killedUnits;

        private PlayerUnit _currentAct;
        private TurnController _turnController;
        private bool _autoMode;

        public Action<PlayerUnit> RegisterNewUnit;
        public Func<Vector2Int, Tile> GetTile;

        public Action<Location> OnNewLocation;
        
        public Action OnTurnEnd;


        public PlayerManager(UIManager UImanager)
        {
            _UImanager = UImanager;
            _UImanager.OnUnitSelection = ChangeSelectedUnit;

            _turnController = new();

            _allUnits = new();
            _killedUnits = new();
            _locations = new();
        }

        public void Act()
        {
            if(_allUnits.Count == 0) AddUnit(new(Vector2Int.zero, 0, 3));

            ClearKilled();

            foreach (var l in _locations) l.Act();

            _UImanager.SetUnitsList(_allUnits);
            _autoMode = false;

            _turnController.Setup(_allUnits);

            SetNextTurn();
        }


        private async void SetNextTurn()
        {
            if (_turnController.ReadyToFinish && !_autoMode)
                _autoMode = await _UImanager.FinishTurnClick();

            _currentAct = _turnController.GetNextTurn();

            if (_currentAct != null) _currentAct.Act(_autoMode);
            else
            {
                OnTurnEnd?.Invoke();
                Debug.Log("Finish turn");
            }
        }
        private async Task<ICommand> ProcessUnitCommand(PlayerUnit unit, List<ICommand> commands)
        {
            var choice = await _UImanager.GetCommand(unit, commands);
            
            switch (choice.ExtraInput)
            {
                case BaseCommand.AdditionalInput.Tile:
                    choice.SetTile(GetTile(unit.ModelPosition));
                    break;
                case BaseCommand.AdditionalInput.Location:
                    SetBuildCommand(unit.ModelPosition, ref choice);
                    break;
                default: break;
            }
            
            return choice;
        }
        private void SetBuildCommand(Vector2Int position, ref ICommand command)
        {
            var existing = _locations.Where(l => l.ModelPosition== position).FirstOrDefault();
            if (existing == null)
            {
                existing = new(position);
                existing.OnBuildComplete = () => AddLocation(existing);
                existing.OnNewUnit = AddUnit;
            }

            if (GetTile(position).CheckBuildRequirements(existing.UnitsLimit)) command.SetLocation(existing);
            else command = null;
        }
        private void AddLocation(Location location)
        {
            if (!_locations.Contains(location))
            {
                _locations.Add(location);
                OnNewLocation?.Invoke(location);
            }
        }
        private void AddUnit(PlayerUnit unit)
        {
            unit.Name = $"PlayerUnit{_allUnits.Count}";
            unit.OnTurnEnd = SetNextTurn;
            unit.RequestCommand = async c => await ProcessUnitCommand(unit, c);
            unit.OnDeath = () => { if (!_killedUnits.Contains(unit)) _killedUnits.Add(unit); };
            
            _allUnits.Add(unit);
            RegisterNewUnit?.Invoke(unit);
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

        public bool ReadyToFinish { get; private set; }

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
            ReadyToFinish = _requiredTurn.Count == 0;
        }

        public PlayerUnit GetNextTurn()
        {
            if (_currentTurn != null && !_currentTurn.FinishedActions)
            {
                if (_currentTurn.HasTask) _autoTurn.Add(_currentTurn);
                else _requiredTurn.Add(_currentTurn);
            }

            _currentTurn = null;

            if (_requiredTurn.Count > 0)
            {
                _currentTurn = _requiredTurn[0];
                _requiredTurn.RemoveAt(0);
                //if (_requiredTurn.Count == 0)
                //{
                //    OnReadyToFinishTurn?.Invoke();
                //}
            }
            else if (_autoTurn.Count > 0)
            {
                _currentTurn = _autoTurn[0];
                _autoTurn.RemoveAt(0);
            }

            ReadyToFinish = _requiredTurn.Count == 0;
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
