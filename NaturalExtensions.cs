using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

static class NaturalExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> series)
        => series == null || !series.Any();

    public static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = true)
        where TAttribute : Attribute
    {
        return
            type?.GetCustomAttributes(typeof(TAttribute), inherit)
                .OfType<TAttribute>()
                .FirstOrDefault();
    }
    public static TAttribute GetAttribute<TAttribute>(this Type type, string member, bool inherit = true)
        where TAttribute : Attribute
    {
        var memberInfos = type.GetMember(member);
        return
            memberInfos.FirstOrDefault(m => m.DeclaringType == type)
                ?.GetCustomAttributes(typeof(TAttribute), inherit)
                .OfType<TAttribute>()
                .FirstOrDefault();
    }

    public static string Join(this IEnumerable<string> strings, string separator, string surround = "")
    {
        var items = strings as IList<string> ?? strings.ToList();
        if (items.Count == 0) return "";
        if (surround == null) surround = "";
        var surroundLength = surround.Length * 2;
        var finalLength = items.Sum(i => i.Length + surroundLength) + separator.Length * (items.Count - 1);
        var stringBuilder = new StringBuilder(finalLength);

        stringBuilder.Append(surround);
        stringBuilder.Append(items[0]);
        stringBuilder.Append(surround);
        if (surround.Length <= 0)
        {
            for (var i = 1; i < items.Count; i++)
            {
                stringBuilder.Append(separator);
                stringBuilder.Append(items[i]);
            }
            return stringBuilder.ToString();
        }
        for (var i = 1; i < items.Count; i++)
        {
            stringBuilder.Append(separator);
            stringBuilder.Append(surround);
            stringBuilder.Append(items[i]);
            stringBuilder.Append(surround);
        }
        return stringBuilder.ToString();
    }
    public static IEnumerable<string> Split(this string value, params string[] separators)
    {
        return value
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(s=>s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));
    }

    public static IEnumerable<Type> GetInheritors(this Type type)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(type.IsAssignableFrom);
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items) collection.Add(item);
    }
    public static ConstructorInfo GetDefaultConstructor(this Type type)
    {
        return type.GetConstructor(Type.EmptyTypes);
    }
}