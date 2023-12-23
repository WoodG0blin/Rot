using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rot
{
    public class TileView : MonoBehaviour
    {
        private Renderer _renderer;
        private Transform _transform;

        [SerializeField] private Transform _selection;
        [SerializeField] private Transform _mask;
        [SerializeField] private Transform _location;
        [SerializeField] private Transform _enemyLocation;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private TextMeshProUGUI _locationMessage;

        private bool _selected;

        private bool selected
        {
            get => _selected;
            set
            {
                _selected = value;
                SetSelection(_selected);
            }
        }


        public void OnClick() => selected = !selected;
        public void Init(Vector3 position, Material material)
        {
            _transform ??= transform;
            _transform.position = position;

            _renderer ??= _transform.GetComponentInChildren<Renderer>();
            _renderer.material = material;

            selected = false;
        }
        public void UpdateTile(bool isAlive, int vitality, BaseLocation location)
        {
            SetMask(!isAlive);
            _message.text = vitality.ToString();
            ShowLocation(location);
        }

        private void ShowLocation(BaseLocation location)
        {
            _location.parent.gameObject.SetActive(location != null);

            _location.gameObject.SetActive(location is PlayerLocation);
            _enemyLocation.gameObject.SetActive(location is EnemyLocation);
            if(location != null) _locationMessage.text = location.Vitality.ToString();
        }

        public void SetSelection(bool selected) =>
            _selection.gameObject.SetActive(selected);

        public void SetMask(bool mask) =>
            _mask.gameObject.SetActive(mask);
    }
}
