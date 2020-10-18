using NaughtyAttributes;

namespace BustaGames.Joystick
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(RectTransform))]
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float ANGLE_THRESHOLD = 22.5f;
        private const float VERTICAL_MIN_ANGLE = 90f - ANGLE_THRESHOLD;
        private const float VERTICAL_MAX_ANGLE = 90f + ANGLE_THRESHOLD;
        private const float HORIZONTAL_MIN_ANGLE = ANGLE_THRESHOLD;
        private const float HORIZONTAL_MAX_ANGLE = 180f - ANGLE_THRESHOLD;
        
        public Vector3 Direction => _input;

        [Tooltip("If the controls will always be visible, or only when an input is detected.")]
        public bool alwaysVisible = true;

        [Tooltip("Configures how far can the handle go away from the center. \n" +
                 "1 means it will reach the border of the background."), Min(0)]
        public float handleRange = 1;

        [Tooltip("Minimum range necessary to trigger an input. \n" +
                 "Value of 0 means it will always detect an input."), Min(0)]
        public float deadZone;

        [Tooltip("Tells which axes will provide with an input value.")]
        public AxisOptions axisOptions = AxisOptions.Both;

        public bool snapX;
        public bool snapY;

        [Space(8f)]
        public bool moveToTouch;

        [ShowIf("moveToTouch")] public bool returnOnRelease;
        [ShowIf("moveToTouch")] public bool followInput;

        [ShowIf("followInput"), Min(0)] public float followThreshold = 1;

        [Space(8f)]
        [SerializeField, Required]
        private RectTransform background;

        [SerializeField, Required] private RectTransform handle;

        private bool _isDown;
        private bool _hasHandle;
        private RectTransform _baseRect;

        private Canvas _canvas;

        private Vector3 _input = Vector2.zero;

        private Vector2 _fixedPosition = Vector2.zero;

        private void Awake()
        {
            _baseRect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();

            followThreshold = Mathf.Abs(followThreshold);
            handleRange = Mathf.Abs(handleRange);
            deadZone = Mathf.Abs(deadZone);

            if (!moveToTouch)
            {
                followInput = false;
                returnOnRelease = false;
            }
        }

        private void Start()
        {
            if (!alwaysVisible)
            {
                background.gameObject.SetActive(false);
            }

            if (_canvas == null)
            {
                Debug.LogError("The Joystick is not placed inside a canvas");
            }

            _fixedPosition = background.position;
            var center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            _hasHandle = handle != null;
            if (_hasHandle)
            {
                handle.anchorMin = center;
                handle.anchorMax = center;
                handle.pivot = center;
                handle.anchoredPosition = Vector2.zero;
            }
            
            _input = Vector3.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (moveToTouch)
            {
                var localPoint = ScreenToWorld(eventData);
                Debug.Log(eventData.position + " " + _baseRect.rect + " " + localPoint);
                background.position = localPoint;
                if (!alwaysVisible)
                {
                    background.gameObject.SetActive(true);
                }
            }

            OnDrag(eventData);
        }

        private Vector3 ScreenToWorld(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_baseRect, eventData.position, eventData.enterEventCamera,
                out var localPoint);
            return localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _isDown = true;

            var position = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, background.position);
            var radius = background.sizeDelta / 2;
            _input = (eventData.position - position) / (radius * _canvas.scaleFactor * handleRange);
            FormatInput();
            HandleInput(radius);
            if (_hasHandle)
            {
                handle.anchoredPosition = _input * radius * handleRange;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!alwaysVisible)
            {
                background.gameObject.SetActive(false);
            }

            if (returnOnRelease)
            {
                background.position = _fixedPosition;
            }

            _isDown = false;
            _input = Vector2.zero;
            if (_hasHandle)
            {
                handle.anchoredPosition = Vector2.zero;
            }
        }

        private void HandleInput(Vector2 radius)
        {
            var magnitude = _input.magnitude;
            var normalised = _input.normalized; // TODO: Avoid calculating normalised every time.
            HandleFollowInput(magnitude, normalised, radius);

            if (magnitude > deadZone)
            {
                if (magnitude > 1)
                {
                    _input = normalised;
                }
            }
            else
            {
                _input = Vector2.zero;
            }
            
            SnapValues();
        }

        private void HandleFollowInput(float magnitude, Vector2 normalised, Vector2 radius)
        {
            if (followInput && magnitude > followThreshold)
            {
                var difference = normalised * (magnitude - followThreshold) * radius;
                background.anchoredPosition += difference;
            }
        }

        private void FormatInput()
        {
            switch (axisOptions)
            {
                case AxisOptions.Horizontal:
                    _input.y = 0;
                    break;
                case AxisOptions.Vertical:
                    _input.x = 0;
                    break;
                case AxisOptions.Both:
                    // do nothing
                    break;
                case AxisOptions.None:
                    _input = Vector3.zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SnapValues()
        {
            var angle = Vector2.Angle(_input, Vector2.up);
            
            if (snapX && _input.x != 0)
            {
                _input.x = angle > HORIZONTAL_MIN_ANGLE && angle < HORIZONTAL_MAX_ANGLE 
                    ? _input.x > 0 ? 1 : -1 
                    : 0;
            }

            if (snapY && _input.y != 0)
            {
                _input.y = angle < VERTICAL_MIN_ANGLE || angle > VERTICAL_MAX_ANGLE 
                    ? _input.y > 0 ? 1 : -1 
                    : 0;
            }
        }
    }
}