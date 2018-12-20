using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleAxis1D : MonoBehaviour
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
            _image.fillAmount = TBInput.GetAxis1D(_button, _controller);
        }

        public void SetButton(TBInput.Button button, TBInput.Controller controller)
        {
            _button = button;
            _controller = controller;

            if (!TBInput.SupportsAxis1D(button, controller))
                transform.parent.gameObject.SetActive(false);
        }
    }

}