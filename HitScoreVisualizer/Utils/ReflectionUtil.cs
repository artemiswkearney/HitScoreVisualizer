using System.Reflection;

namespace HitScoreVisualizer.Utils
{
    static class ReflectionUtil
    {
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            var prop = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop.SetValue(obj, value);
        }

        public static T GetPrivateField<T>(object obj, string fieldName)
        {
            var prop = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var value = prop.GetValue(obj);
            return (T)value;
        }

        public static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop.SetValue(obj, value, null);
        }

        public static void SetPrivateFieldBase(object obj, string fieldName, object value)
        {
            var prop = obj.GetType().BaseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop.SetValue(obj, value);
        }

        public static T GetPrivateFieldBase<T>(object obj, string fieldName)
        {
            var prop = obj.GetType().BaseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var value = prop.GetValue(obj);
            return (T)value;
        }

        public static void SetPrivatePropertyBase(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().BaseType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop.SetValue(obj, value, null);
        }

        public static void InvokePrivateMethod(object obj, string methodName, object[] methodParams)
        {
            MethodInfo dynMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod.Invoke(obj, methodParams);
        }

        public static T getPrivateField<T>(this object obj, string fieldName) =>
            GetPrivateField<T>(obj, fieldName);
        public static void setPrivateField(this object obj, string fieldName, object value) =>
            SetPrivateField(obj, fieldName, value);
        public static T getPrivateFieldBase<T>(this object obj, string fieldName) =>
            GetPrivateFieldBase<T>(obj, fieldName);
        public static void setPrivateFieldBase(this object obj, string fieldName, object value) =>
            SetPrivateFieldBase(obj, fieldName, value);
        public static void setPrivateProperty(this object obj, string propertyName, object value) =>
            setPrivateProperty(obj, propertyName, value);
        public static void setPrivatePropertyBase(this object obj, string propertyName, object value) =>
            setPrivatePropertyBase(obj, propertyName, value);
        public static void invokePrivateMethod(this object obj, string methodName, object[] methodParams) =>
            InvokePrivateMethod(obj, methodName, methodParams);
    }
}