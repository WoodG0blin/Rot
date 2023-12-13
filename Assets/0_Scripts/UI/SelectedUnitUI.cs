using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class SelectedUnitUI : AwaitableView
    {
        [SerializeField] private GameObject _commandButtonPrefab;
        [SerializeField] private Transform _commandPanel;

        private ICommand _selectedCommand;
        private CancellationTokenSource _cancelTS;

        public void SetSelectedUnit(PlayerUnit unit)
        {
            _cancelTS ??= new CancellationTokenSource();

            // Displaying icons and stats
        }
        public async Task<ICommand> ChooseCommandFrom(List<ICommand> available)
        {
            foreach(var command in available)
            {
                var commandButton = Instantiate(_commandButtonPrefab, _commandPanel).GetComponent<CommandButtonUI>();
                commandButton.SetButton(command.Icon, command.Name, () => SetCommand(command));
            }

            return await this ? _selectedCommand : null;
        }
        public void AbortAwait()
        {
            _cancelTS.Cancel();
            currentAwaiter?.Finish(false);
        }


        private async void SetCommand(ICommand command)
        {
            _cancelTS.Cancel();
            _selectedCommand = command;
            
            if(await TrySetAdditionalInput(_cancelTS.Token)) currentAwaiter?.Finish(true);
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
