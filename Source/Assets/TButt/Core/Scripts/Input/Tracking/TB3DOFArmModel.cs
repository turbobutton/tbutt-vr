using UnityEngine;
using UnityEngine.VR;
using System.Collections;

namespace TButt
{
    public class TB3DOFArmModel : MonoBehaviour
    {
        public static TB3DOFArmModel instance = null;

        [Header("Pivot Values")]
        [Tooltip("The distance between the head and the shoulder.")]
        public Vector3 shoulderPosition = new Vector3(0.22f, 0f, 0f);
        [Tooltip("The distance between the elbow and the shoulder.")]
        public Vector3 elbowPosition = new Vector3(0f, -0.4f, 0.025f);
        [Tooltip("The distance between the hand and the elbow.")]
        public Vector3 handPosition = new Vector3(0f, 0f, 0.2f);

        private Transform _handTransform;
        private Transform _shoulderTransform;
        private Transform _elbowTransform;
        private Transform _pointerSourceTransform;

        private float _pointerSourceAngle = 20;

        //positive = right, negative = left
        private float shoulderPositionX = 0;

        private bool _centered = false;

        public void Initialize()
        {
            instance = this;

            shoulderPositionX = shoulderPosition.x;

            // Set up the hand and various containers.
            _shoulderTransform = new GameObject().transform;
            _elbowTransform = new GameObject().transform;
            _handTransform = new GameObject().transform;
            _pointerSourceTransform = new GameObject().transform;

#if UNITY_EDITOR
            _shoulderTransform.gameObject.name = "Arm";
            _elbowTransform.gameObject.name = "Elbow";
            _handTransform.gameObject.name = "Hand";
            _pointerSourceTransform.gameObject.name = "PointerSource";
#endif
        }

        void Start()
        {
            OnHandednessChanged(TBInput.Get3DOFHandedness());

            // Attach the arm to the camera container.
            _shoulderTransform.SetParent(TBCameraRig.instance.transform);
            _shoulderTransform.localRotation = Quaternion.identity;
            _shoulderTransform.localPosition = shoulderPosition;

            // Attach the elbow to the arm pivot.
            _elbowTransform.SetParent(_shoulderTransform);
            _elbowTransform.localPosition = elbowPosition;

            // Attach the hand to the elbow.
            _handTransform.SetParent(_elbowTransform);
            _handTransform.localPosition = handPosition;

            _pointerSourceTransform.MakeZeroedChildOf(_handTransform);
            SetPointerSourceRotation();
            _shoulderTransform.localScale = Vector3.one;
        }

        void OnEnable()
        {
            TBInput.Events.OnHandednessChanged += OnHandednessChanged;
        }

        void OnDisable()
        {
            TBInput.Events.OnHandednessChanged -= OnHandednessChanged;
        }

        void Update()
        {
            if (!_elbowTransform || !_shoulderTransform)
            {
                return;
            }

            _elbowTransform.localRotation = TBInput.GetRotation(TBInput.Controller.Mobile3DOFController);

            _shoulderTransform.position = Vector3.Lerp(_shoulderTransform.position, GetTargetPosition(), TBInput.GetAcceleration(TBInput.Controller.Mobile3DOFController).magnitude * Time.unscaledDeltaTime);
        }

        void OnHandednessChanged(TBInput.Mobile3DOFHandedness handedness)
        {
            if (_centered)
                return;

            if (handedness == TBInput.Mobile3DOFHandedness.Left)
                shoulderPosition = new Vector3(-shoulderPositionX, shoulderPosition.y, shoulderPosition.z);
            else
                shoulderPosition = new Vector3(shoulderPositionX, shoulderPosition.y, shoulderPosition.z);

            Debug.Log("Arm model updated handedness to " + handedness);

            _shoulderTransform.position = GetTargetPosition();
        }

        Vector3 GetTargetPosition()
        {
            return TBCameraRig.instance.GetCenter().position + TBCameraRig.instance.GetCenter().right * shoulderPosition.x;
        }

        public void ToggleCenteredAlignment(bool on)
        {
            if (on)
            {
                shoulderPosition = new Vector3(0f, shoulderPosition.y, shoulderPosition.z);
                _shoulderTransform.localPosition = shoulderPosition;
            }
            else
            {
                OnHandednessChanged(TBInput.Get3DOFHandedness());
            }
            _centered = on;
        }

        public Transform GetPointerSourceTransform()
        {
            return _pointerSourceTransform;
        }

        public void SetPointerSourceRotation()
        {
            _pointerSourceTransform.localRotation = Quaternion.AngleAxis(_pointerSourceAngle, _pointerSourceTransform.right);
        }

        /// <summary>
        ///  Returns a reference to the hand transform.
        /// </summary>
        /// <returns></returns>
        public Transform GetHandTransform()
        {
            return _handTransform;
        }
    }
}
