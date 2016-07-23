using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(Slider))]
    public class GuiSliderBind : GuiBindableBase
    {
        [SerializeField]
        private BindPath _bindPath;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(val => UpdateSource(val));
        }

        public override void DataUpdated(object data)
        {
            switch (_bindPath)
            {
                case BindPath.Value:
                    _slider.value = (float?)data ?? 0f;
                    break;

                case BindPath.MinValue:
                    _slider.minValue = (float?)data ?? 0f;
                    break;

                case BindPath.MaxValue:
                    _slider.maxValue = (float?)data ?? 0f;
                    break;
            }
        }

        public override object GetGuiComponent()
        {
            return _slider;
        }

        enum BindPath
        {
            Value,
            MinValue,
            MaxValue
        }
    }
}