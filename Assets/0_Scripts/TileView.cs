using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private Color _base;
    [SerializeField] private Color _selected;

    private Renderer _renderer;

    private void Start()
    {
        _renderer = transform.GetComponent<Renderer>();
        SetSelection(false);
    }

    public void SetSelection(bool selected)
    {
        _renderer.sharedMaterial.color = selected ? _selected : _base;
    }
}
