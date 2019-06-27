using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Sample
{
    /// <summary>
    /// Basic script for handling hand presence for controllers with thumb, grip, and index finger detection.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class TBSampleHandAnimation : MonoBehaviour
    {
        public TBInput.Controller controller = TBInput.Controller.Active;

        private Animator _animator;

        private int _indexHash;
        private int _thumbHash;
        private int _gripHash;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _indexHash = Animator.StringToHash("Index");
            _gripHash = Animator.StringToHash("Grip");
            _thumbHash = Animator.StringToHash("Thumb");
        }

        private void OnEnable()
        {
            TBCore.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            TBCore.OnUpdate -= OnUpdate;
        }

        void OnUpdate()
        {
            _animator.SetFloat(_indexHash, TBInput.GetFinger(TBInput.Finger.Index, controller));
            _animator.SetFloat(_thumbHash, TBInput.GetFinger(TBInput.Finger.Thumb, controller));
            _animator.SetFloat(_gripHash, TBInput.GetFinger(TBInput.Finger.Grip, controller));
        }
    }
}