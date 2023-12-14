using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class SelectedUnitUI : AwaitableView
    {
        [SerializeField] private GameObject _commandButtonPrefab;
        [SerializeField] private Transform _commandPanel;
        [SerializeField] private TextMeshProUGUI _name;

        private ICommand _selectedCommand;
        private CancellationTokenSource _cancelTS;

        public void SetSelectedUnit(PlayerUnit unit)
        {
            _name.text = unit.Name;
            // Displaying icons and stats
        }
        public async Task<ICommand> ChooseCommandFrom(List<ICommand> available)
        {
            ClearCommandPanel();

            foreach(var command in available)
            {
                var commandButton = Instantiate(_commandButtonPrefab, _commandPanel).GetComponent<CommandButtonUI>();
                commandButton.SetButton(command.Icon, command.Name, () => SetCommand(command));
            }

            AbortAwait();
            bool res = await this;
            currentAwaiter = null;
            return res ? _selectedCommand : null;
        }
        public void AbortAwait()
        {
            _cancelTS?.Cancel();
            currentAwaiter?.Finish(false);
        }

        private void ClearCommandPanel()
        {
            int children = _commandPanel.childCount;
            if (children == 0) return;
            for(int i = children; i > 0; i--)
            {
                var c = _commandPanel.GetChild(i-1);
                c.SetParent(null);
                GameObject.Destroy(c.gameObject);
            }
        }
        private async void SetCommand(ICommand command)
        {
            _cancelTS?.Cancel();
            _cancelTS = new();
            _selectedCommand = command;

            if (await TrySetAdditionalInput(_cancelTS.Token)) currentAwaiter?.Finish(true);
        }
        private async Task<bool> TrySetAdditionalInput(CancellationToken cancellation)
        {
            bool result = true;

            switch (_selectedCommand.ExtraInput)
            {
                case BaseCommand.AdditionalInput.Position:
                    Vector2Int? targetPosition = new Vector2Int(0,0); //replace with await with token - position
                    if (targetPosition != null) _selectedCommand.SetTargetPosition(targetPosition.Value);
                    else result = false;
                    break;
                case BaseCommand.AdditionalInput.Target:
                    IDamagable target = null; //replace with await with token - target
                    if (target != null) _selectedCommand.SetTarget(target);
                    else result = false;
                    break;
                default: break;
            }

            if (cancellation.IsCancellationRequested) result = false;
            return result;
        }
    }
}
