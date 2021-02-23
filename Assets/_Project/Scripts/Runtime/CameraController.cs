using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;
using System;
using System.Collections;

namespace BHPanorama
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ScrollUp();

        [DllImport("__Internal")]
        private static extern void ScrollDown();
#endif

        [SerializeField] private HotspotManager _hotspotManager;

        [SerializeField] private SpriteRenderer _cameraFadeSpriteRenderer = default;

        [SerializeField] private float _mouseSensitivity = 125.0f;
        [SerializeField] private float _clampAngle = 80.0f;
        [SerializeField] private bool _invertMouse = true;

        [SerializeField] private Material _skyboxMaterial = default;
        [SerializeField] private float fadeTime = 1f;

        private float _defaultFov = 60f;
        [SerializeField] private float _minFov = 20f;
        [SerializeField] private float _maxFov = 60f;
        [SerializeField] private float _zoomSensitivity = 10f;

        private Camera _camera;

        private float _rotY = 0.0f; // rotation around the up/y axis
        private float _rotX = 0.0f; // rotation around the right/x axis

        private bool _enableInput = true;
        private bool _dayTime = true;

        void Start()
        {
            _camera = GetComponent<Camera>();

            _cameraFadeSpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
            _cameraFadeSpriteRenderer.gameObject.SetActive(false);

            MessageSystem.Subscribe<OpenMenuEvent>(HandleOpenMenu);
            MessageSystem.Subscribe<OpenMenuEvent>(HandleCloseMenu);
            MessageSystem.Subscribe<ChangeTimeEvent>((e) => HandleChangeTimeEvent(e.Day));
        }

        private void HandleChangeTimeEvent(bool day)
        {
            _dayTime = day;
            ChangeTexture(_hotspotManager.CurrentHotspot);
        }

        private void Update()
        {
            if (_enableInput && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                if (_invertMouse)
                {
                    mouseX *= -1;
                    mouseY *= -1;
                }

                _rotY += mouseX * _mouseSensitivity * Time.deltaTime;
                _rotX += mouseY * _mouseSensitivity * Time.deltaTime;

                _rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);

                Quaternion localRotation = Quaternion.Euler(_rotX, _rotY, 0.0f);
                transform.rotation = localRotation;
            }

#if !UNITY_EDITOR && UNITY_WEBGL
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ScrollUp();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ScrollDown();
        }
#endif
        }

        private void HandleCloseMenu(OpenMenuEvent e)
        {
            _enableInput = true;
        }

        private void HandleOpenMenu(OpenMenuEvent e)
        {
            _enableInput = false;
        }

        private void Zoom()
        {
            float fov = Camera.main.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
            fov = Mathf.Clamp(fov, _minFov, _maxFov);
            Camera.main.fieldOfView = fov;
        }

        private void ResetRotation(Hotspot hotspot)
        {
            transform.localRotation = hotspot.transform.localRotation;
            Vector3 rot = transform.localRotation.eulerAngles;
            Camera.main.fieldOfView = _defaultFov;

            _rotY = rot.y;
            _rotX = rot.x;
        }

        private void ChangeTexture(Hotspot hotspot)
        {
            if (_dayTime)
            {
                _skyboxMaterial.mainTexture = hotspot.HotspotTexture;
            }
            else
            {
                _skyboxMaterial.mainTexture = hotspot.HotspotTextureNight;
            }
        }

        public void SetCurrentHotSpot(Hotspot hotspot)
        {
            transform.position = hotspot.transform.position;

            ChangeTexture(hotspot);

            RenderSettings.skybox.SetFloat("_Rotation", hotspot.HotspotRotation);

            ResetRotation(hotspot);

            MessageSystem.Trigger(new EndMovingToHotspotEvent(hotspot));
        }

        private void FinishCameraTransition()
        {
            Camera.main.fieldOfView = _defaultFov;
            _cameraFadeSpriteRenderer.gameObject.SetActive(false);
        }

        public void MoveTo(Hotspot targetHotspot)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToCoroutine(targetHotspot));
        }

        private IEnumerator MoveToCoroutine(Hotspot targetHotspot)
        {
            transform.DOLookAt(targetHotspot.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            _camera.DOFieldOfView(20f, fadeTime);
            _cameraFadeSpriteRenderer.gameObject.SetActive(true);
            _cameraFadeSpriteRenderer.DOFade(1f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            SetCurrentHotSpot(targetHotspot);
            transform.DOMove(targetHotspot.transform.position, 0f);
            _cameraFadeSpriteRenderer.DOFade(0f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            FinishCameraTransition();
        }
    }
}
