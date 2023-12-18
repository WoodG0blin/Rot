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

        private Vector2Int _initialPosition;
        private ICommand _selectedCommand;
        private Action cancel;

        public Func<Vector2Int, Action, int, Task<Path>> RequestPathFrom;
        public Func<Action, Task<IDamagable>> RequestTarget;

        public void SetSelectedUnit(PlayerUnit unit)
        {
            _name.text = unit.Name;
            _initialPosition = unit.ModelPosition;
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
            cancel?.Invoke();
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
            cancel?.Invoke();
            cancel = null;
            _selectedCommand = command;

            if (await TrySetAdditionalInput()) currentAwaiter?.Finish(true);
        }
        private async Task<bool> TrySetAdditionalInput()
        {
            bool result = true;

            switch (_selectedCommand.ExtraInput)
            {
                case BaseCommand.AdditionalInput.Position:
                    var path = await RequestPathFrom(_initialPosition, cancel, (_selectedCommand as MoveCommand).MaxSpeed);
                    if (path != null) _selectedCommand.SetPath(path);
                    else result = false;
                    break;
                case BaseCommand.AdditionalInput.Target:
                    IDamagable target = await RequestTarget(cancel);
                    if (target != null) _selectedCommand.SetTarget(target);
                    else result = false;
                    break;
                default: break;
            }

            return result;
        }
    }
}
