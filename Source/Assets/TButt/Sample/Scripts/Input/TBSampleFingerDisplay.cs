using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TButt;

namespace TButt.Sample
{
    public class TBSampleFingerDisplay : MonoBehaviour
    {
        public TBInput.Controller controller;

        public Sprite closedFist;
        public Sprite fingerGun;
        public Sprite pointing;
        public Sprite open;
        public Sprite thumbDown;
        public Sprite pinch;
        public Sprite thumbsUp;

        [Header("Image References")]
        public Image _icon;
        public Image _thumb;
        public Image _index;
        public Image _grip;

        private float _thumbWeight = 0;
        private float _indexWeight = 0;
        private float _gripWeight = 0;

        private void Start()
        {
            if (TBInput.GetControllerModel(controller) == VRController.None)
            {
                gameObject.SetActive(false);
                Debug.LogWarning("Finger chart was disabled because no controller was found for " + controller + ". Is that input type enabled in Input Settings?");
                return;
            }

            if (!TBInput.SupportsFinger(TBInput.Finger.Thumb, controller))
            {
                _thumb.enabled = false;
                _thumb.transform.parent.gameObject.SetActive(false);
            }
            if (!TBInput.SupportsFinger(TBInput.Finger.Index, controller))
            {
                _index.transform.parent.gameObject.SetActive(false);
                _index.enabled = false;
            }
            if (!TBInput.SupportsFinger(TBInput.Finger.Grip, controller))
            {
                _grip.transform.parent.gameObject.SetActive(false);
                _grip.enabled = false;
            }

            if (controller == TBInput.Controller.RHandController)
                ToggleHandedness();
        }

        private void Update()
        {
            if (_thumb.enabled)
            {
                _thumbWeight = TBInput.GetFinger(TBInput.Finger.Thumb, controller);
                _thumb.fillAmount = _thumbWeight;
            }            
            if (_index.enabled)
            {
                _indexWeight = TBInput.GetFinger(TBInput.Finger.Index, controller);
                _index.fillAmount = _indexWeight;
            }
            if (_grip.enabled)
            {
                _gripWeight = TBInput.GetFinger(TBInput.Finger.Grip, controller);
                _grip.fillAmount = _gripWeight;
            }
            else
            {
                _gripWeight = _indexWeight;
            }

            ResolveIcon();
        }

        public void ToggleHandedness()
        {
            _icon.transform.localScale = new Vector3(-1, 1, 1);
        }

        private void ResolveIcon()
        {
            if(_gripWeight > 0.5f)
            {
                if (_indexWeight == 0)
                {
                    if (_thumbWeight == 0)
                        _icon.sprite = fingerGun;
                    else
                        _icon.sprite = pointing;
                }
                else
                {
                    if (_thumbWeight == 0)
                        _icon.sprite = thumbsUp;
                    else
                        _icon.sprite = closedFist;
                }
            }
            else
            {
                if(_indexWeight > 0.1f)
                {
                    if(_thumbWeight > 0.1f)
                    {
                        _icon.sprite = pinch;
                    }
                }
                else
                {
                    if(_thumbWeight > 0)
                    {
                        _icon.sprite = thumbDown;
                    }
                    else
                    {
                        _icon.sprite = open;
                    }
                }
            }
        }
    }
}