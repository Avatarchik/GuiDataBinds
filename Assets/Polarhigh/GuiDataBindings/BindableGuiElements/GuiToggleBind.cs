using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(Toggle))]
    public class GuiToggleBind : GuiBindableBase
    {
        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(val => UpdateSource(val));
        }

        public override object GetGuiComponent()
        {
            return _toggle;
        }

        public override void DataUpdated(object data)
        {
            _toggle.isOn = (bool?) data ?? false;
        }
    }
}