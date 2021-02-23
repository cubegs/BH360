using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace BHPanorama
{
    public class PointOfInterest : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Canvas _poiCanvas = default;
        [SerializeField] private EventTrigger _poiTrigger = default;
        [SerializeField] private Button _poiCloseButton = default;

        [Header("PointOfInterest State")]
        [SerializeField] private Hotspot[] _hotspots = default;

        [Header("Renderer Settings")]
        [SerializeField] private SpriteRenderer _poiIconSpriteRenderer = default;

        public Canvas PoiCanvas { get => _poiCanvas; private set => _poiCanvas = value; }
        public Hotspot[] Hotspots { get => _hotspots; private set => _hotspots = value; }

        public void Initialize()
        {
            AddPointerClickTrigger();
            _poiCloseButton.onClick.AddListener(HandlePointOfInterestCloseClick);
            ClosePointOfInterest();

            MessageSystem.Subscribe<EndMovingToHotspotEvent>((e) => UpdatePoiDirection());
        }

        public void UpdatePoiDirection()
        {
            Transform target = Camera.main.transform;
            Transform poi = _poiIconSpriteRenderer.transform;

            Vector3 canvasLookDir = transform.position - new Vector3(target.position.x, transform.position.y, target.position.z);

            if (canvasLookDir != Vector3.zero)
            {
                PoiCanvas.transform.rotation = Quaternion.LookRotation(canvasLookDir);
            }

            Vector3 poiLookDir = poi.position - new Vector3(target.position.x, poi.position.y, target.position.z);

            if (poiLookDir != Vector3.zero)
            {
                poi.rotation = Quaternion.LookRotation(poiLookDir);
            }
        }

        public void OpenPointOfInterest()
        {
            _poiCanvas.gameObject.SetActive(true);

            MessageSystem.Trigger(new PointOfInterestOpenEvent(this));
        }

        public void ClosePointOfInterest()
        {
            _poiCanvas.gameObject.SetActive(false);

            MessageSystem.Trigger(new PointOfInterestCloseEvent(this));
        }

        private void AddPointerClickTrigger()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(HandlePointOfInterestClick);
            _poiTrigger.triggers.Add(entry);
        }

        private void HandlePointOfInterestCloseClick()
        {
            ClosePointOfInterest();
        }

        private void HandlePointOfInterestClick(BaseEventData eventData)
        {
            OpenPointOfInterest();
        }
    }
}