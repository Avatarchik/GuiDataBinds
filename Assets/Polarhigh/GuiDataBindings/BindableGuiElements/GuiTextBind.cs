using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(Text))]
    public class GuiTextBind : GuiBindableBase
    {
        [SerializeField]
        private BindPath _bindPath = BindPath.Text;

        Text _text;

        // Unity callbacks

        void Awake()
        {
            _text = GetComponent<Text>();
        }

        public override object GetGuiComponent()
        {
            return _text;
        }

        public override void DataUpdated(object data)
        {
            switch (_bindPath)
            {
                case BindPath.Text:
                    _text.text = (string) data;
                    break;

                case BindPath.Font:
                    _text.font = (Font) data;
                    break;

                case BindPath.FontStyle:
                    _text.fontStyle = (FontStyle?) data ?? FontStyle.Normal;
                    break;

                case BindPath.LineSpacing:
                    _text.lineSpacing = (float?) data ?? 0;
                    break;
            }
        }

        enum BindPath
        {
            Text,
            Font,
            FontStyle,
            LineSpacing
        }
    }
}
