namespace RPG_pro.Basics
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class LegacyNameAttribute : Attribute
    {
        public LegacyNameAttribute(params string[] names)
        {
            Names = names ?? throw new ArgumentNullException(nameof(names));
        }
        public static IEnumerable<string> GetLegacyNamesOfType(Type type)
        {
            foreach (LegacyNameAttribute attribute in type.GetCustomAttributes(false).Cast<LegacyNameAttribute>())
            {
                foreach (string legacyName in attribute.Names)
                {
                    yield return legacyName;
                }
            }
            yield break;
        }
        public readonly string[] Names;
    }
}