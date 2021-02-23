using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotspotButton : Button
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _number;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _typeIcon;

    [SerializeField] private Hotspot _hotspot;

    public TMP_Text Label { get => _label; set => _label = value; }
    public TMP_Text Number { get => _number; set => _number = value; }
    public Image Icon { get => _icon; set => _icon = value; }
    public Hotspot Hotspot { get => _hotspot; set => _hotspot = value; }
    public Image TypeIcon { get => _typeIcon; set => _typeIcon = value; }

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
