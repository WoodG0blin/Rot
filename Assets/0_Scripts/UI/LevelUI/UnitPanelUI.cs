using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class UnitPanelUI : MonoBehaviour
    {
        [SerializeField] private Button _selection;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _icon;

        public Action OnClick;
        public void SetUnit(string name, Sprite icon)
        {
            _name.text = name;
            _icon.sprite = icon;
            _selection.onClick.AddListener(() => OnClick?.Invoke());
        }

        private void OnDisable() => _selection.onClick.RemoveAllListeners();
    }
}
