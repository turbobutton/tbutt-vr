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

        private void Start()
        {
            if (TBInput.GetControllerModel(controller) == VRController.None)
            {
                Debug.LogWarning("Input chart was disabled because no controller was found for " + controller + ". Is that input type enabled in Input Settings?");
                gameObject.SetActive(false);
                return;
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