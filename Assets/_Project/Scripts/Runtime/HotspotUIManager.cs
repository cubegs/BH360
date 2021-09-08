using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BHPanorama
{
    public class HotspotUIManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private HotspotManager _hotspotManager = default;
        [SerializeField] private GameObject _leftHotspotButtonsContainer = default;
        [SerializeField] private GameObject _rightHotspotButtonsContainer = default;
        [SerializeField] private Hotspot _hotspotHome = default;

        [Space(10)]
        [Header("UI Componets")]
        [SerializeField] private Button _homeButton = default;
        [SerializeField] private TMP_Text _currentHotspotTMPText = default;

        [SerializeField] private Button _dayButton = default;
        [SerializeField] private TMP_Text _dayTMP = default;

        [SerializeField] private Button _nightButton = default;
        [SerializeField] private TMP_Text _nightTMP = default;

        [Space(10)]
        [Header("General Settings")]
        [SerializeField] private bool _isMainMenuOpen = false;
        [SerializeField] private GameObject _hotspotButtonPrefab = default;
        [SerializeField] private float _showMenuDuration = default;

        private ColorBlock _buttonActiveColors = default;
        private ColorBlock _buttonInactiveColors = default;
        private List<Button> _hotspotButtonList = default;

        public void Start()
        {
            _buttonActiveColors = _dayButton.colors;
            _buttonInactiveColors = _nightButton.colors;

            _hotspotButtonList = new List<Button>();

            _homeButton.onClick.AddListener(MenuClickHandler);
            _dayButton.onClick.AddListener(DayButtonClickHandler);
            _nightButton.onClick.AddListener(NightButtonClickHandler);

            MessageSystem.Subscribe<HotspotInitializeEvent>((e) => AddButton(e.Hotspot));
            MessageSystem.Subscribe<StartMovingToHotspotEvent>((e) => HandleStartMovingToHotspot(e.Hotspot));

            OpenMenu();

            if (!_hotspotManager.CameraController.IsDayTime)
            {
                NightButtonClickHandler();
            }

            //        _menuHomeSprite.transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        }

        private void NightButtonClickHandler()
        {
            _dayTMP.color = Color.white;
            _nightTMP.color = Color.black;
            _dayButton.colors = _buttonInactiveColors;
            _nightButton.colors = _buttonActiveColors;

            MessageSystem.Trigger(new ChangeTimeEvent(false));
        }

        private void DayButtonClickHandler()
        {
            _dayTMP.color = Color.black;
            _nightTMP.color = Color.white;
            _dayButton.colors = _buttonActiveColors;
            _nightButton.colors = _buttonInactiveColors;

            MessageSystem.Trigger(new ChangeTimeEvent(true));
        }

        private void HandleStartMovingToHotspot(Hotspot hotspot)
        {
            if (_hotspotManager.CanMove)
            {
                CloseMenu();
                _currentHotspotTMPText.text = hotspot.Label;
            }
        }

        private void MenuClickHandler()
        {
            if (_hotspotManager.CanMove)
            {
                if (_isMainMenuOpen)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }

        private void OpenMenu()
        {
            if (!_isMainMenuOpen)
            {
                MessageSystem.Trigger(new StartMovingToHotspotEvent(_hotspotHome));

                _isMainMenuOpen = true;

                RectTransform _leftHotspotRect = _leftHotspotButtonsContainer.GetComponent<RectTransform>();

                _leftHotspotRect.anchoredPosition = new Vector2(_leftHotspotRect.anchoredPosition.x - _leftHotspotRect.rect.width, _leftHotspotRect.anchoredPosition.y);
                _leftHotspotRect.DOAnchorPosX(0, _showMenuDuration);

                RectTransform _rightHotspotRect = _rightHotspotButtonsContainer.GetComponent<RectTransform>();

                _rightHotspotRect.anchoredPosition = new Vector2(_rightHotspotRect.anchoredPosition.x + _rightHotspotRect.rect.width, _rightHotspotRect.anchoredPosition.y);
                _rightHotspotRect.DOAnchorPosX(0, _showMenuDuration);

                _homeButton.GetComponent<RectTransform>().DOAnchorPosY(-150f, _showMenuDuration);

                MessageSystem.Trigger(new OpenMenuEvent());
            }
        }

        private void CloseMenu()
        {
            if (_isMainMenuOpen)
            {
                _isMainMenuOpen = false;

                RectTransform _leftHotspotRect = _leftHotspotButtonsContainer.GetComponent<RectTransform>();

                _leftHotspotRect.anchoredPosition = new Vector2(0f, _leftHotspotRect.anchoredPosition.y);
                _leftHotspotRect.DOAnchorPosX(_leftHotspotRect.anchoredPosition.x - _leftHotspotRect.rect.width, _showMenuDuration);

                RectTransform _rightHotspotRect = _rightHotspotButtonsContainer.GetComponent<RectTransform>();

                _rightHotspotRect.anchoredPosition = new Vector2(0f, _rightHotspotRect.anchoredPosition.y);
                _rightHotspotRect.DOAnchorPosX(_rightHotspotRect.anchoredPosition.x + _rightHotspotRect.rect.width, _showMenuDuration);

                _homeButton.GetComponent<RectTransform>().DOAnchorPosY(30f, _showMenuDuration);

                MessageSystem.Trigger(new CloseMenuEvent());
            }
        }

        private void AddButton(Hotspot hotspot)
        {
            GameObject parent = _leftHotspotButtonsContainer;

            if (hotspot.Number > _hotspotManager.HotspotList.Length / 2)
            {
                parent = _rightHotspotButtonsContainer;
            }

            GameObject buttonGo = Instantiate(_hotspotButtonPrefab, parent.transform);
            HotspotButton hotspotButton = buttonGo.GetComponent<HotspotButton>();
            hotspotButton.SetHotspot(hotspot);

            _hotspotButtonList.Add(hotspotButton);
        }
    }
}
