using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Sample
{

    public class TBSampleControllerSelectorContainer : MonoBehaviour
    {
        private bool _subscribed;

        private void OnEnable()
        {
            if(TBInput.GetActiveControlType() == TBInput.ControlType.None)
            {
                TBInput.Events.OnControlTypeChanged += OnControlTypeChanged;
                _subscribed = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if(_subscribed)
                TBInput.Events.OnControlTypeChanged -= OnControlTypeChanged;
        }

        private void OnControlTypeChanged(TBInput.ControlType type)
        {
            gameObject.SetActive(false);
        }
    }
}