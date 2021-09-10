using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;


public static class DynamicExtensions
{
    /// <example>
    /// var pattern = new Regex("(?&lt;success&gt;true)");
    /// var result = pattern.MatchInto(new{Success="success"},"true");
    /// Assert(result.Success=="true");
    /// </example>
    public static dynamic MatchInto(this System.Text.RegularExpressions.Regex regex, object mapping, string text)
    {
        var mappingObject = MappingObject.From(mapping);
        var match = regex.Match(text);
        if (!match.Success) return null;
        var result = new Dictionary<string,string>();
        foreach (var pair in mappingObject)
        {
            var groupName = pair.Value;
            if (string.IsNullOrEmpty(groupName)) continue;
            try
            {
                var groupValue = match.Groups[groupName].Value;
                result.Add(pair.Key,groupValue);
            }
            catch
            {
                result.Add(pair.Key,null);
            }
        }
        return result.ToObject();
    }

    /// <example>
    /// var dict = new Dictionary&lt;string,object&gt;{{"Success",true}};
    /// var result = dict.ToObject();
    /// Assert(result.Success==true);
    /// </example>
    public static dynamic ToObject<TItem>(this IDictionary<string,TItem> dict)
    {
        return dict.Aggregate(new ExpandoObject() as IDictionary<string, object>,
            (a, p) => { a.Add(p.Key,p.Value); return a; });
    }
}
public class MappingObject : IEnumerable<KeyValuePair<string,string>>
{
    private readonly IDictionary<string, string> _keyValueMapping;

    private MappingObject(IDictionary<string,string> keyValueMapping)
    {
        _keyValueMapping = keyValueMapping;
    }

    public static MappingObject From(object mappingObject)
    {
        var type = mappingObject.GetType();
        var pairs = type
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(mappingObject)?.ToString());
        return new MappingObject(pairs);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _keyValueMapping.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
