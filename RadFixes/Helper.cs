using HarmonyLib;

namespace RadFixes
{
    internal static class Helper
    {
        public static T GetPrivateField<T>(this object obj, string field)
        {
            return (T)Traverse.Create(obj).Field(field).GetValue();
        }

        public static void SetPrivateField(this object obj, string field, object value)
        {
            Traverse.Create(obj).Field(field).SetValue(value);
        }

        public static void SetPrivateField<T>(string field, object value)
        {
            Traverse.Create(typeof(T)).Field(field).SetValue(value);
        }
    }
}
