using System;
using System.Reflection;
using UnityEngine;

namespace Assets.Polarhigh.GuiDataBindings.BindableGuiElements
{
    /// <summary>
    /// Поддерживается биндинг только от источника к графическому интерфейсу
    /// </summary>
    public class GuiBindableUniversal : GuiBindableBase
    {
        [SerializeField]
        private string _bindPath;

        [SerializeField]
        private Component _bindComponent;

        private Action<object> _setValue;


        private void Awake()
        {
            string[] propertiesHierarchy = _bindPath.Split('.');

            if (propertiesHierarchy.Length > 0)
                _setValue = CreateUpdateDelegate(propertiesHierarchy);
            else
                throw new Exception("Invalid bind path");
        }

        public override object GetGuiComponent()
        {
            return _bindComponent;
        }

        public override void DataUpdated(object data)
        {
            _setValue(data);
        }

        private Action<object> CreateUpdateDelegate(string[] propsHierarhy)
        {
            object curObject = _bindComponent;
            Type curType;

            for (int i = 0; i < propsHierarhy.Length - 1; i++)
            {
                curType = curObject.GetType();

                PropertyInfo pi = curType.GetProperty(propsHierarhy[i]);
                if (pi != null)
                {
                    curObject = pi.GetGetMethod().Invoke(curObject, null);
                    continue;
                }

                FieldInfo fi = curType.GetField(propsHierarhy[i]);
                if (fi != null)
                {
                    curObject = fi.GetValue(curObject);
                    continue;
                }

                throw new Exception("Invalid property path");
            }
            
            curType = curObject.GetType();
            string lastProp = propsHierarhy[propsHierarhy.Length - 1];

            PropertyInfo setProp = curType.GetProperty(lastProp);
            if (setProp != null)
                return CreateLambdaForProp(setProp, curObject);

            FieldInfo setField = curType.GetField(lastProp);
            if (setField != null)
                return CreateLambdaForField(setField, curObject);

            throw new Exception("Invalid propery path");
        }

        private static Action<object> CreateLambdaForProp(PropertyInfo pi, object obj)
        {
            return val => pi.GetSetMethod().Invoke(obj, new[] {val});
        }

        private static Action<object> CreateLambdaForField(FieldInfo fi, object obj)
        {
            return val => fi.SetValue(obj, val);
        }
    }
}
