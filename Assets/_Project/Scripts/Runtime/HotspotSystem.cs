﻿using UnityEngine;

namespace BHPanorama
{
    public class HotspotSystem : MonoBehaviour
    {
        [SerializeField] private HotspotManager _hotspotManager = default;

        void Start()
        {
            _hotspotManager.Initialize();
        }
    }
}