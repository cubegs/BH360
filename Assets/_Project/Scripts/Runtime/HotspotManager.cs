using System;
using UnityEngine;

public class HotspotManager : MonoBehaviour
{
    [SerializeField] private GameObject _hotspotContainer = default;

    [SerializeField] private Hotspot _currentHotspot = default;
    [SerializeField] private PointOfInterest[] _poiList = default;
    [SerializeField] private Hotspot[] _hotspotList = default;

    [SerializeField] private CameraController _cameraController = default;
    [SerializeField] private bool _canMove = true;

    public Hotspot CurrentHotspot { get => _currentHotspot; set => _currentHotspot = value; }
    public bool CanMove { get => _canMove; set => _canMove = value; }
    public Hotspot[] HotspotList { get => _hotspotList; set => _hotspotList = value; }


    private void Update()
    {
#if UNITY_EDITOR_WIN
        if (CurrentHotspot != null)
        {
//            CurrentHotspot.UpdateLinkedHotspots();
        }
#endif
    }

    public void Initialize()
    {
        _poiList = FindObjectsOfType<PointOfInterest>();
        HotspotList = _hotspotContainer.GetComponentsInChildren<Hotspot>();

        foreach (PointOfInterest poi in _poiList)
        {
            poi.Initialize();
        }

        for (int i = 0; i < HotspotList.Length; i++)
        {
            Hotspot hotspot = HotspotList[i];
            hotspot.Number = i + 1;
            hotspot.Initialize();
        }

        MessageSystem.Subscribe<StartMovingToHotspotEvent>((e) => HandleStartMovingToHotspot(e.Hotspot));
        MessageSystem.Subscribe<PointOfInterestOpenEvent>((e) => HandlePointOfInterestOpen(e.PointOfInterest));
        MessageSystem.Subscribe<PointOfInterestCloseEvent>((e) => HandlePointOfInterestClose(e.PointOfInterest));
        MessageSystem.Subscribe<EndMovingToHotspotEvent>((e) => HandleEndMovingToHotspot());

        _cameraController.SetCurrentHotSpot(CurrentHotspot);
    }

    private void HandlePointOfInterestOpen(PointOfInterest pos)
    {

    }

    private void HandlePointOfInterestClose(PointOfInterest poi)
    {

    }

    private void HandleStartMovingToHotspot(Hotspot newHotspot)
    {
        if (CanMove)
        {
            CanMove = false;

            CurrentHotspot = newHotspot;
            _cameraController.MoveTo(newHotspot);
        }
    }

    private void HandleEndMovingToHotspot()
    {
        CanMove = true;

        UpdateHotSpots();
        CurrentHotspot.gameObject.SetActive(false);
        CurrentHotspot.UpdateLinkedHotspots();
    }

    private void UpdateHotSpots()
    {
        for (int i = 0; i < HotspotList.Length; i++)
        {
            if (CurrentHotspot.HotspotParametersList == null || CurrentHotspot.HotspotParametersList.Length == 0)
            {
                HotspotList[i].gameObject.SetActive(true);
            }
            else
            {
                HotspotList[i].gameObject.SetActive(false);
            }
        }
    }
}