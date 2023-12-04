using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    private Renderer _renderer;

    [SerializeField] private Material _base;
    [SerializeField] private Material _select;
    
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


    private void Start()
    {
        _renderer = transform.GetComponentInChildren<Renderer>();
        selected = false;
    }

    public void SetSelection(bool selected) =>
        _renderer.material = selected ? _select : _base;

}
