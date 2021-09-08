using UnityEngine;

namespace BHPanorama
{
    public class HotspotManager : MonoBehaviour
    {
        public static bool SHOW_UI = false;

        [Header("Scene References")]
        [SerializeField] private GameObject _hotspotContainer = default;
        [SerializeField] private CameraController _cameraController = default;

        [Header("Hotspot State")]
        [SerializeField] private Hotspot _currentHotspot = default;
        [SerializeField] private PointOfInterest[] _poiList = default;
        [SerializeField] private Hotspot[] _hotspotList = default;
        [SerializeField] private bool _canMove = true;

        public Hotspot CurrentHotspot { get => _currentHotspot; set => _currentHotspot = value; }
        public bool CanMove { get => _canMove; set => _canMove = value; }
        public Hotspot[] HotspotList { get => _hotspotList; set => _hotspotList = value; }
        public CameraController CameraController { get => _cameraController; set => _cameraController = value; }

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
            MessageSystem.Subscribe<EndMovingToHotspotEvent>((e) => HandleEndMovingToHotspot());

            CameraController.SetCurrentHotSpot(CurrentHotspot);
        }

        private void HandleStartMovingToHotspot(Hotspot newHotspot)
        {
            if (CanMove)
            {
                CanMove = false;

                CurrentHotspot = newHotspot;
                CameraController.MoveTo(newHotspot);
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
}