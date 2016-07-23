using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(Button))]
    public class GuiButtonBind : GuiBindableBase
    {
        [SerializeField]
        private BindPath _bindPath = BindPath.Text;

        Button _button;

        // Unity callbacks

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        // IUiBindable interface

        public override object GetGuiComponent()
        {
            return _button;
        }

        public override void DataUpdated(object data)
        {
            switch (_bindPath)
            {
                case BindPath.Text:
                    _button.GetComponent<GUIText>().text = (string) data;
                    break;

                default:
                    throw new Exception("Invalid bind path");
            }
        }

        enum BindPath
        {
            Text,
            OnlyEvents
        }
    }
}