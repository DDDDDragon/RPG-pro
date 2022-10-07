namespace RPG_pro.Basics
{
    internal class TypeCaching
    {
        public static event Action OnClear;
        public static void Clear()
        {
            OnClear?.Invoke();
        }
    }
}