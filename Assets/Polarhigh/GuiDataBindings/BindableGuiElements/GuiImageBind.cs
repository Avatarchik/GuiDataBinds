using UnityEngine;
using UnityEngine.UI;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    [RequireComponent(typeof(Image))]
    public class GuiImageBind : GuiBindableBase
    {
        [SerializeField]
        private BindPath _bindPath = BindPath.SourceImage;

        private Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
        }


        public override object GetGuiComponent()
        {
            return _image;
        }

        public override void DataUpdated(object data)
        {
            switch (_bindPath)
            {
                case BindPath.SourceImage:
                    _image.sprite = (Sprite) data ?? new Sprite();
                    break;

                case BindPath.Color:
                    _image.color = (Color?) data ?? new Color();
                    break;

                case BindPath.FillAmount:
                    _image.fillAmount = (float?) data ?? 0f;
                    break;
            }
        }

        public enum BindPath
        {
            SourceImage,
            Color,
            FillAmount
        }
    }
}
