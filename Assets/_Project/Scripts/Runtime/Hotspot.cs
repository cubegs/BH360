using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BHPanorama
{
    public class Hotspot : MonoBehaviour
    {
        [Header("Hotspot Data")]
        [SerializeField] private int _number = default;
        [SerializeField] private string _label = default;
        [SerializeField] private Color _color = default;
        [SerializeField] private Sprite _menuSprite = default;

        [Space(10)]
        [Header("UI Components References")]
        [SerializeField] private TMP_Text _numberTMP = default;
        [SerializeField] private SpriteRenderer _hotspotSR = default;

        [SerializeField] private Texture _hotspotTexture = default;
        [SerializeField] private Texture _hotspotTextureNight = default;

        [SerializeField] private EventTrigger _spriteTrigger = default;

        [Space(10)]
        [Header("Hotspot Settings")]
        [SerializeField] private Transform _hotspotRoot = default;
        [SerializeField] private float _hotspotRotation = default;

        [SerializeField] private PointOfInterest[] _pointOfInterestList = default;
        [SerializeField] private HotspotParameters[] _HotspotParametersList = default;

        private Vector3 _originalScale = default;
        private Vector3 _originalPosition = default;

        public float HotspotRotation { get => _hotspotRotation; private set => _hotspotRotation = value; }
        public string Label { get => _label; private set => _label = value; }
        public int Number { get => _number; set => _number = value; }
        public HotspotParameters[] HotspotParametersList { get => _HotspotParametersList; private set => _HotspotParametersList = value; }
        public Transform HotspotRoot { get => _hotspotRoot; private set => _hotspotRoot = value; }
        public EventTrigger SpriteTrigger { get => _spriteTrigger; private set => _spriteTrigger = value; }
        public Texture HotspotTexture { get => _hotspotTexture; private set => _hotspotTexture = value; }
        public Texture HotspotTextureNight { get => _hotspotTextureNight; private set => _hotspotTextureNight = value; }
        public Color Color { get => _color; private set => _color = value; }
        public Sprite MenuSprite { get => _menuSprite; private set => _menuSprite = value; }

        public void Initialize()
        {
            _originalPosition = transform.position;
            _originalScale = transform.localScale;

            _hotspotSR.color = new Color(Color.r, Color.g, Color.b, 1.0f);
            _numberTMP.text = Number.ToString();

            AddPointerClickTrigger();
            SetPointOfInterestList();

            MessageSystem.Trigger(new HotspotInitializeEvent(this));
            MessageSystem.Subscribe<EndMovingToHotspotEvent>((e) => UpdateHotspotDirection());
        }

        public void UpdateHotspotDirection()
        {
            Transform target = Camera.main.transform;

            ResetPosition();

            Vector3 lookDir = transform.position - new Vector3(target.position.x, transform.position.y, target.position.z);
            //        Vector3 lookDir = transform.position - target.position;

            if (lookDir != Vector3.zero)
            {
                _hotspotRoot.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        public void UpdateLinkedHotspots()
        {
            foreach (HotspotParameters hotspotParameters in HotspotParametersList)
            {
                hotspotParameters.HotSpot.GetComponent<Hotspot>().ResetPosition();
                hotspotParameters.HotSpot.gameObject.SetActive(true);
                hotspotParameters.HotSpot.transform.localScale *= hotspotParameters.ScaleOffset;
                hotspotParameters.HotSpot.transform.position += hotspotParameters.PositionOffset;
            }
        }

        public void ResetPosition()
        {
            transform.position = _originalPosition;
            transform.localScale = _originalScale;
        }

        public void NavigateToHotspot()
        {
            MessageSystem.Trigger(new StartMovingToHotspotEvent(this));
        }

        private void SetPointOfInterestList()
        {
            List<PointOfInterest> _tempPoiList = new List<PointOfInterest>();

            foreach (PointOfInterest poi in FindObjectsOfType<PointOfInterest>())
            {
                if (System.Array.IndexOf(poi.Hotspots, this) >= 0)
                {
                    _tempPoiList.Add(poi);
                }
            }

            _pointOfInterestList = _tempPoiList.ToArray();
        }

        private void AddPointerClickTrigger()
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(HandleHotspotClick);
            SpriteTrigger.triggers.Add(entry);
        }

        private void HandleHotspotClick(BaseEventData eventData)
        {
            NavigateToHotspot();
        }
    }
}