using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MixMatch2.Shared.Interfaces;

public interface IMetadata : IXmlSerializable
{
    List<MetadataTag> Tags { get; set; }
    public void WriteMetaToFile(FileResult file);
}

[Serializable]
public class MetadataTag : ISerializable
{
    public enum MetadataTagValueTypes
    {
        String = 0,
        Path = 1,
        Number = 2,
        MixMatchTags = 3,
        Nested = 4,
        Error = -1
    }

    // No, this is not unsafe. Every constructor for this class sets the public facing Value object,
    // which either sets this or throws an error on null.
    private object _value = null!;
    private MetadataTagValueTypes _selfValue;
    public MetadataTagValueTypes SelfValue
    {
        get => _selfValue;
        set
        {
            _ = value;
        }
    }

    public string? Key { get; set; }
    public dynamic? Value
    {
        get
        {
            return _selfValue switch
            {
                MetadataTagValueTypes.String => (string)_value,
                MetadataTagValueTypes.Number => (double)_value,
                MetadataTagValueTypes.Path => Path.GetFullPath((string)_value),
                MetadataTagValueTypes.MixMatchTags => (MixMatchTag)_value,
                MetadataTagValueTypes.Nested => (MetadataTag)_value,
                MetadataTagValueTypes.Error => throw new MetadataTypeException(_value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _value = value;
            _selfValue = value switch
            {
                string => Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute)
                    ? MetadataTagValueTypes.Path
                    : MetadataTagValueTypes.String,
                double or float or int or long => MetadataTagValueTypes.Number,
                MixMatchTag => MetadataTagValueTypes.MixMatchTags,
                MetadataTag => MetadataTagValueTypes.Nested,
                _ => MetadataTagValueTypes.Error,
            };
            if (_selfValue == MetadataTagValueTypes.Error)
            {
                throw new MetadataTypeException(_value);
            }
        }
    }

    public MetadataTag()
    {
        Key = null;
        Value = null;
    }

    public MetadataTag(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("key", Key, typeof(string));
        info.AddValue("value", Value, typeof(object));
        info.AddValue("type", SelfValue, typeof(MetadataTagValueTypes));
    }

    public MetadataTag(SerializationInfo info, StreamingContext context)
    {
        SelfValue = (MetadataTagValueTypes)(info.GetValue("type", typeof(MetadataTagValueTypes))
            ?? MetadataTagValueTypes.Error);
        Key = info.GetValue("key", typeof(string)) as string ?? "";
        Value = info.GetValue("value", typeof(object)) as object ?? null;

    }
}

public class MetadataTypeException : Exception
{
    public string Error { get; }

    public MetadataTypeException(object value)
    {
        Error = "Invalid type " + value.GetType() +
                ", Expected string, number, MixMatchTags, or MetadataTag.";
    }
}

public class MixMatchTag
{
    private readonly Dictionary<string, string> _customTags;
    public MixMatchTag()
    {
        _customTags = new Dictionary<string, string>();
    }

    public MixMatchTag(Dictionary<string, string> customTags)
    {
        _customTags = customTags;
    }


    public string? GetTag(string tagName)
    {
        return _customTags.TryGetValue(tagName, out var value) ? value : null;
    }

    public void SetTag(string tagName, string value)
    {
        _customTags[tagName] = value;
    }
}