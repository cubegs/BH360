using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotSystem : MonoBehaviour
{
    [SerializeField] private HotspotManager _hotspotManager = default;

    void Start()
    {
        _hotspotManager.Initialize();
    }
}