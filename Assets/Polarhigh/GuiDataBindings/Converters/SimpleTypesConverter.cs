using System;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.Converters
{
    public class SimpleTypesConverter : ValueConverterBase
    {
        [SerializeField]
        private ConvertType _convertToGui;

        [SerializeField]
        private ConvertType _convertToSource;

        public override object ConvertFromSourceToGui(object value)
        {
            return ConvertToType(_convertToGui, value);
        }

        public override object ConvertFromGuiToSource(object value)
        {
            return ConvertToType(_convertToSource, value);
        }

        private object ConvertToType(ConvertType convertType, object value)
        {
            switch (convertType)
            {
                case ConvertType.String:
                    return System.Convert.ToString(value);

                case ConvertType.Int:
                    return System.Convert.ToInt32(value);

                case ConvertType.Float:
                    return (float)System.Convert.ToDouble(value);

                case ConvertType.Double:
                    return System.Convert.ToDouble(value);

                case ConvertType.Bool:
                    return System.Convert.ToBoolean(value);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        enum ConvertType
        {
            String,
            Int,
            Float,
            Double,
            Bool
        }
    }
}