using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleTouch : MonoBehaviour
    {
        private TBInput.Button _button;
        private TBInput.Controller _controller;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            if (TBInput.GetTouch(_button, _controller))
                _image.color = Color.green;
            else
                _image.color = Color.white;
        }
        public void SetButton(TBInput.Button button, TBInput.Controller controller)
        {
            _button = button;
            _controller = controller;

            if (!TBInput.SupportsTouch(button, controller))
                gameObject.SetActive(false);
        }
    }

}