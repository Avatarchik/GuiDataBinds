using System;
using System.Reflection;

namespace Assets.Polarhigh.GuiDataBindings.TypeExtensions
{
    public static class TypeExtensionsClass
    {
        public static object GetProperyOrFieldValue(this Type type, object obj, string propertyOrField)
        {
            //Type type = obj.GetType();

            PropertyInfo evntPropInfo = type.GetProperty(propertyOrField);
            if (evntPropInfo != null)
                return evntPropInfo.GetGetMethod().Invoke(obj, null);

            FieldInfo evntFieldInfo = type.GetField(propertyOrField);
            if (evntFieldInfo != null)
                return evntFieldInfo.GetValue(obj);

            return null;
        }
    }
}
