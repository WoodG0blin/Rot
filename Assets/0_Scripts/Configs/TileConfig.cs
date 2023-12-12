using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rot
{
    [Serializable]
    internal class TileConfig
    {
        [SerializeField] private TileTypes _type;
        [SerializeField] private int _probability;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private bool _isPassable = true;
        [SerializeField, Range(1, 10)] private int _passingCost;

        public TileTypes Type => _type;
        public int Probability => _probability;
        public Material BaseMaterial => _baseMaterial;
        public int PassingCost => _isPassable ? _passingCost : 1000;
    }
}
