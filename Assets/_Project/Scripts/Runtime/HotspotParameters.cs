using UnityEngine;
using System;

namespace BHPanorama
{
    [Serializable]
    public class HotspotParameters
    {
        [SerializeField] private GameObject _hotSpot = default;
        [SerializeField] private float _scaleOffset = 1;
        [SerializeField] private Vector3 _positionOffset = default;

        public GameObject HotSpot { get => _hotSpot; private set => _hotSpot = value; }
        public float ScaleOffset { get => _scaleOffset; private set => _scaleOffset = value; }
        public Vector3 PositionOffset { get => _positionOffset; private set => _positionOffset = value; }
    }
}