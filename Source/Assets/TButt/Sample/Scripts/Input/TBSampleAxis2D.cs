using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleAxis2D : MonoBehaviour
    {
        private TBInput.Button _button;
        private TBInput.Controller _controller;

        private void Update()
        {
            Vector2 buttonPosition = TBInput.GetAxis2D(_button, _controller);
            Vector3 newPosition = new Vector3(buttonPosition.x * 8f, buttonPosition.y * 8f, 0f);
            transform.localPosition = newPosition;
        }

        public void SetButton(TBInput.Button button, TBInput.Controller controller)
        {
            _button = button;
            _controller = controller;

            if (!TBInput.SupportsAxis2D(button, controller))
                transform.parent.gameObject.SetActive(false);
        }
    }

}