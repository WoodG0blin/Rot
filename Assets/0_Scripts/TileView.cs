using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileView : MonoBehaviour
{
    private Renderer _renderer;
    private Transform _transform;

    [SerializeField] private Transform _selection;
    [SerializeField] private Transform _mask;
    [SerializeField] private TextMeshProUGUI _message;
    
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
    public void UpdateTile(bool isAlive, int vitality)
    {
        SetMask(!isAlive);
        _message.text = vitality.ToString();
    }

    public void SetSelection(bool selected) =>
        _selection.gameObject.SetActive(selected);

    public void SetMask(bool mask) =>
        _mask.gameObject.SetActive(mask);
}
