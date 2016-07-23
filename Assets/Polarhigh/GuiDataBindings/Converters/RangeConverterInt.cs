using System;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.Converters
{
    /// <summary>
    /// Конвертер приводит число из установленного диапазона к числу от 0 до 1
    /// </summary>
    public class RangeConverterInt : ValueConverterBase
    {
        public int SourceRangeMin
        {
            get { return _sourceRangeMin; }
            set { _sourceRangeMin = value; }
        }

        public int SourceRangeMax
        {
            get { return _sourceRangeMax; }
            set { _sourceRangeMax = value; }
        }

        [SerializeField]
        private int _sourceRangeMin;

        [SerializeField]
        private int _sourceRangeMax;

        public override object ConvertFromSourceToGui(object value)
        {
            int intVal = (int)value;

            if (intVal < _sourceRangeMin)
                return 0f;

            if (intVal > _sourceRangeMax)
                return 1f;

            return (float)(intVal - _sourceRangeMin) / (_sourceRangeMax - _sourceRangeMin);
        }

        public override object ConvertFromGuiToSource(object value)
        {
            throw new NotImplementedException();
        }
    }
}
