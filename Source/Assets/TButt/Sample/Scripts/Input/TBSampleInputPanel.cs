using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleInputPanel : MonoBehaviour
    {
        public TBInput.Controller controller;
        public List<TBInput.Button> buttons;

        public Text controllerLabel;

        public TBSampleInputRow rowPrefab;

        private TBSampleInputRow _activeRow;
        private RectTransform _rect;
        private float _rowOffset = -42;



        private IEnumerator Start()
        {
            if (TBInput.GetControllerModel(controller) == VRController.None)
            {
                Debug.Log("Input chart was disabled because no controller was found for " + controller + ". This means that the associated ControlType is disabled in TButt's Input Settings, or no compatible controller exists for the current platform.");
                gameObject.SetActive(false);
                yield break;
            }

            controllerLabel.text = controller.ToString() + " | " + TBInput.GetControllerModel(controller).ToString();
            _rect = GetComponent<RectTransform>();

            for (int i = 0; i < buttons.Count; i++)
            {
                if (TBInput.SupportsVirtualButton(buttons[i], controller))
                {
                    CreateDisplayRow(buttons[i]);
                }
            }

            ConformHeights();

            TBInput.ControlType assocaitedControlType = TBInput.ControlType.None;

            switch(controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    assocaitedControlType = TBInput.ControlType.HandControllers;
                    break;
                case TBInput.Controller.Mobile3DOFController:
                    assocaitedControlType = TBInput.ControlType.Mobile3DOFController;
                    break;
                case TBInput.Controller.Gamepad:
                    assocaitedControlType = TBInput.ControlType.Gamepad;
                    break;
                case TBInput.Controller.ClickRemote:
                    assocaitedControlType = TBInput.ControlType.ClickRemote;
                    break;
            }

            if (assocaitedControlType != TBInput.GetActiveControlType())
            {
                gameObject.GetComponent<Canvas>().enabled = false;

                while (TBInput.GetActiveControlType() == TBInput.ControlType.None)
                {
                    yield return null;
                }

                if (assocaitedControlType == TBInput.GetActiveControlType())
                    gameObject.GetComponent<Canvas>().enabled = true;
            }
        }

        void CreateDisplayRow(TBInput.Button button)
        {
            _activeRow = (TBSampleInputRow)Instantiate(rowPrefab, transform, false);
            _activeRow.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, _rowOffset);
            _activeRow.SetButton(button, controller);

            _rowOffset -= 26;

            if(_rect.sizeDelta.y < -_rowOffset - 12)
                _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, -_rowOffset - 12);

        }

        void ConformHeights()
        {
            TBSampleInputPanel[] panels = FindObjectsOfType<TBSampleInputPanel>();

            for(int i = 0; i < panels.Length; i++)
            {
                float height = panels[i].gameObject.GetComponent<RectTransform>().sizeDelta.y;
                if (height > _rect.sizeDelta.y)
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, height);
                else
                    panels[i].gameObject.GetComponent<RectTransform>().sizeDelta = _rect.sizeDelta;
            }
        }
    }
}