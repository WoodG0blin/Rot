using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rot
{
    public class ChoicePanelUI : AwaitableView
    {
        [SerializeField] Transform _choicePanel;
        [SerializeField] GameObject _choiceButtonPrefab;

        private IDamagable _currentChoice;

        public async Task<IDamagable> GetChoiceAt(List<IDamagable> choices, Vector2 position, Action cancel)
        {
            if (choices == null || choices.Count == 0) return null;

            cancel += AbortAwait;
            PlaceAt(position);
            SetButtons(choices);
            await this;
            cancel -= AbortAwait;
            return _currentChoice;
        }


        private void AbortAwait() => currentAwaiter?.Finish();
        private void ProcessClick(IDamagable target)
        {
            _currentChoice = target;
            currentAwaiter?.Finish();
            gameObject.SetActive(false);
        }
        private void PlaceAt(Vector2 position)
        {
            gameObject.SetActive(true);
            _choicePanel.position = position;
            gameObject.GetComponent<Button>().onClick.AddListener(() => ProcessClick(null));
        }
        private void SetButtons(List<IDamagable> choices)
        {
            Clear();
            foreach (var c in choices)
            {
                var b = Instantiate(_choiceButtonPrefab, _choicePanel);
                b.GetComponentInChildren<TextMeshProUGUI>().text = c.Name;
                b.GetComponent<Button>().onClick.AddListener(() => ProcessClick(c));
            }
            _currentChoice = null;
        }
        private void Clear()
        {
            int children = _choicePanel.childCount;
            if (children == 0) return;
            for (int i = children; i > 0; i--)
            {
                var c = _choicePanel.GetChild(i - 1);
                c.SetParent(null);
                GameObject.Destroy(c.gameObject);
            }
        }
    }
}
