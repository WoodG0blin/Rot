using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class CommandButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;

        public void SetButton(Sprite sprite, string name, Action onClick)
        {
            _icon.sprite= sprite;
            _name.text= name;
            _button.onClick.AddListener(
                () => { _button.onClick.RemoveAllListeners(); onClick(); } );
        }
    }
}
