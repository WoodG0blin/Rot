using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class SelectedUnitUI : AwaitableView
    {
        [SerializeField] private Button _finishTurn;


        public void SetSelectedUnit(PlayerUnit unit)
        {

        }


        protected override void OnStartAwait()
        {
            _finishTurn.enabled = true;
        }


        private void Awake()
        {
            _finishTurn.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            _finishTurn.enabled = false;
            clickAwaiter?.Finish(true);
        }
        private void OnDestroy()
        {
            _finishTurn.onClick.RemoveAllListeners();
        }
    }
}
