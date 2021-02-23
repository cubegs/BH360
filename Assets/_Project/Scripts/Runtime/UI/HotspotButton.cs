using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BHPanorama
{
    public class HotspotButton : Button
    {
        [SerializeField] private TMP_Text _label = default;
        [SerializeField] private TMP_Text _number = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private Image _typeIcon = default;

        [SerializeField] private Hotspot _hotspot = default;

        public TMP_Text Label { get => _label; private set => _label = value; }
        public TMP_Text Number { get => _number; private set => _number = value; }
        public Image Icon { get => _icon; private set => _icon = value; }
        public Hotspot Hotspot { get => _hotspot; private set => _hotspot = value; }
        public Image TypeIcon { get => _typeIcon; private set => _typeIcon = value; }

        protected override void Start()
        {
            base.Start();

            onClick.AddListener(HandleButtonClick);
        }

        private void HandleButtonClick()
        {
            MessageSystem.Trigger(new StartMovingToHotspotEvent(Hotspot));
        }

        public void SetHotspot(Hotspot hotspot)
        {
            Hotspot = hotspot;
            Label.text = hotspot.Label.ToUpper();
            Number.text = hotspot.Number.ToString();
            Icon.color = hotspot.Color;
            TypeIcon.sprite = hotspot.MenuSprite;
        }
    }
}
