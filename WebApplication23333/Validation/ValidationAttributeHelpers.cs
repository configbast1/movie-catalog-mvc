namespace WebApplication23333.Validation;

// спільний хелпер: додає data-атрибут лише якщо його ще немає
internal static class ValidationAttributeHelpers
{
    public static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }
}
