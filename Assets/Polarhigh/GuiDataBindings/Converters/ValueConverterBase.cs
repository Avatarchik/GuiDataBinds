using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.Converters
{
    public abstract class ValueConverterBase : MonoBehaviour
    {
        public abstract object ConvertFromSourceToGui(object value);
        public abstract object ConvertFromGuiToSource(object value);
    }
}
