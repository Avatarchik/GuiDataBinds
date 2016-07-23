using Assets.Polarhigh.GuiDataBindings.Converters;

namespace Assets.Scripts.Todos.Converters
{
    public class ItemsCountConverter : ValueConverterBase
    {
        public override object ConvertFromSourceToGui(object value)
        {
            int count = (int) value;

            if (count > 1)
                return count + " tasks left";

            if (count == 1)
                return "One task left";

            return "All tasks completed!";
        }

        public override object ConvertFromGuiToSource(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
