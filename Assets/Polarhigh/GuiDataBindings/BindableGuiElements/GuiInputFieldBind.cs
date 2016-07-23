using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(InputField))]
    public class GuiInputFieldBind : GuiBindableBase
    {
        private InputField _inputField;

        [SerializeField]
        private UpdateSourceMode _updateSourceMode = UpdateSourceMode.OnEndEdit;

        // Unity callbacks

        private void Awake()
        {
            _inputField = GetComponent<InputField>();

            switch (_updateSourceMode)
            {
                case UpdateSourceMode.OnValueChanged:
                    _inputField.onValueChanged.AddListener(UpdateSource);
                    break;

                case UpdateSourceMode.OnEndEdit:
                    _inputField.onEndEdit.AddListener(UpdateSource);
                    break;
            }
        }

        public override object GetGuiComponent()
        {
            return _inputField;
        }

        public override void DataUpdated(object data)
        {
            _inputField.text = (string) data;
        }

        enum UpdateSourceMode
        {
            OnValueChanged,
            OnEndEdit
        }
    }
}
