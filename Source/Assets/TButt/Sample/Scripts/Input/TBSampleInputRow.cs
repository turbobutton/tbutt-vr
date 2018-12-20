using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleInputRow : MonoBehaviour
    {
        private Text _label;
        private TBSamplePress _press;
        private TBSampleTouch _touch;
        private TBSampleAxis1D _axis1D;
        private TBSampleAxis2D _axis2D;

        public void SetButton(TBInput.Button button, TBInput.Controller controller)
        {
            _label = GetComponentInChildren<Text>();
            _press = GetComponentInChildren<TBSamplePress>();
            _touch = GetComponentInChildren<TBSampleTouch>();
            _axis1D = GetComponentInChildren<TBSampleAxis1D>();
            _axis2D = GetComponentInChildren<TBSampleAxis2D>();

            _label.text = button.ToString();
            _press.SetButton(button, controller);
            _touch.SetButton(button, controller);
            _axis1D.SetButton(button, controller);
            _axis2D.SetButton(button, controller);
        }
    }

}