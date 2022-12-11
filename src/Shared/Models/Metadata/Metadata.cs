using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace MixMatch2.Shared.Models.Metadata;

public class Metadata : IEnumerable, IEnumerator
{
    /// <summary>
    /// Returns a boolean if the object has the same keys and values,
    /// or if the object is a reference to the same object.
    /// </summary>
    /// <param name="obj"> The object to test.</param>
    /// <returns> A boolean if the objects are the same. </returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Metadata)obj);
    }

    /// <summary>
    /// Checks to see if the passed in object is the same as the current one.
    /// </summary>
    /// <param name="other"> The other Metadata object to compare against. </param>
    /// <returns> True if the objects are the same, false otherwise. </returns>
    protected bool Equals(Metadata other)
    {
        return this == other;
    }

    /// <summary>
    /// Gets the hash code of this object.
    /// </summary>
    /// <returns> An int representing a unique hash code. </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(42, _contents);
    }

    private int _enumPos = -1;
    object IEnumerator.Current => Current;

    private enum MetadataTagValueTypes
    {
        String = 0,
        Number = 2,
        MixMatchTags = 3,
        Nested = 4,
        Error = -1
    }
    private readonly Dictionary<string, Tuple<MetadataTagValueTypes, object>> _contents;

    /// <summary>
    /// Serializes this metadata to an XElement.
    /// </summary>
    /// <returns>An XElement that represents this object.</returns>
    /// <exception cref="MetadataTypeException"> One of the values in this metadata is invalid. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Reinstall .net, this error shouldn't be possible to hit. </exception>
    public XElement Serialize()
    {
        var content = (from kvp in _contents
            let type = kvp.Value.Item1 switch
            {
                MetadataTagValueTypes.String => new XAttribute("type", "string"),
                MetadataTagValueTypes.Number => new XAttribute("type", "number"),
                MetadataTagValueTypes.MixMatchTags => new XAttribute("type", "mixMatchTag"),
                MetadataTagValueTypes.Nested => new XAttribute("type", "nested"),
                MetadataTagValueTypes.Error => throw new MetadataTypeException(kvp.Value.Item2),
                _ => throw new ArgumentOutOfRangeException()
            }
            let key = new XAttribute("key", kvp.Key)
            let tagContent = kvp.Value.Item1 switch
            {
                MetadataTagValueTypes.String => kvp.Value.Item2,
                MetadataTagValueTypes.Number => kvp.Value.Item2,
                MetadataTagValueTypes.MixMatchTags => ((MixMatchTag)kvp.Value.Item2).Serialize(),
                MetadataTagValueTypes.Nested => ((Metadata)kvp.Value.Item2).Serialize(),
                MetadataTagValueTypes.Error => throw new MetadataTypeException(kvp.Value.Item2),
                _ => throw new ArgumentOutOfRangeException()
            }
            select new XElement("Tag", type, key, tagContent)).ToArray();
        return new XElement("Metadata", content);
    }

    /// <summary>
    /// Deserializes the XElement into a new Metadata Object
    /// </summary>
    /// <remarks> INCOMPLETE </remarks>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static Metadata Deserialize(XElement xml)
    {
        return null;
    }

    /// <summary>
    /// Creates a new, empty Metadata object
    /// </summary>
    public Metadata()
    {
        _contents = new();
    }
    private Metadata(Dictionary<string, Tuple<MetadataTagValueTypes, object>> contents)
    {
        _contents = contents;
    }

    /// <summary>
    /// Gets a value from a given key, or creates a new kvp if it didn't previously exist.
    /// </summary>
    /// <param name="s"> The key to search. </param>
    /// <returns> A dynamic object, either a string, double, Metadata, or MixMatchTag object. </returns>
    /// <exception cref="MetadataTypeException"> Invalid metadata type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Reinstall .net, this error shouldn't be possible to hit. </exception>
    public dynamic this[string s]
    {
        get
        {
            return _contents[s].Item1 switch
            {
                MetadataTagValueTypes.String => (string)this[s],
                MetadataTagValueTypes.Number => (double)this[s],
                MetadataTagValueTypes.Nested => (Metadata)this[s],
                MetadataTagValueTypes.MixMatchTags => (MixMatchTag)this[s],
                MetadataTagValueTypes.Error => throw new MetadataTypeException(_contents[s].Item2),
                _ => throw new ArgumentOutOfRangeException(nameof(_contents))
            };
        }
        set
        {
            var type = value switch
            {
                string => MetadataTagValueTypes.String,
                double or int or float or long => MetadataTagValueTypes.Number,
                Metadata => MetadataTagValueTypes.Nested,
                MixMatchTag => MetadataTagValueTypes.MixMatchTags,
                _ => throw new MetadataTypeException(value),
            };
            _contents[s] = new Tuple<MetadataTagValueTypes, object>(type, value);
        }
    }

    public static bool operator ==(Metadata a, Metadata b)
    {
        return 
            a
                .Cast<KeyValuePair<string, dynamic>>()
                .All(kvp => a[kvp.Key].Equals(b[kvp.Key]));
    }

    public static bool operator !=(Metadata a, Metadata b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Adds a new element to the metadata <br/>
    /// This can also be accomplished by using the [] operator.
    /// </summary>
    /// <param name="key"> The new key to add. </param>
    /// <param name="value"> The new value to add. </param>
    public void Add(string key, object value)
    {
        this[key] = value;
    }

    /// <summary>
    /// IEnumerable implementation. Do not call.
    /// </summary>
    /// <returns> A boolean if the enumerable is still ongoing. </returns>
    public bool MoveNext()
    {
        _enumPos++;
        return _enumPos < _contents.Keys.Count;
    }
    /// <summary>
    /// IEnumerable implementation. Do not call.
    /// </summary>
    public void Reset()
    {
        _enumPos = -1;
    }

    /// <summary>
    /// The current object in an enumerable loop.
    /// </summary>
    public KeyValuePair<string, dynamic> Current
    {
        get
        {
            if (_enumPos < 0 || _enumPos > _contents.Keys.Count) throw new InvalidOperationException();
            var key = _contents.Keys.ToArray()[_enumPos];
            return new KeyValuePair<string, dynamic>(key, this[key]);
        }
    }

    /// <summary>
    /// Ienumerable implementation. Do not call.
    /// </summary>
    /// <returns> A new IEnumerator to use for enumeration. </returns>
    public IEnumerator GetEnumerator()
    {
        return new Metadata(_contents);
    }
}

public class MetadataTypeException : Exception
{
    public string Error { get; }

    /// <summary>
    /// Creates a new error message from a value, displaying the type passed in.
    /// </summary>
    /// <param name="value"> The value that was attempted to add to Metadata. </param>
    public MetadataTypeException(object value)
    {
        Error = "Invalid type " + value.GetType() +
                ", Expected string, number, MixMatchTags, or MetadataTag.";
    }
}
public class MixMatchTag : IEnumerable, IEnumerator
{
    private readonly Dictionary<string, string> _customTags;
    private int _enumPos = -1;
    /// <summary>
    /// Creates a new, empty MixMatchTag.
    /// </summary>
    public MixMatchTag()
    {
        _customTags = new Dictionary<string, string>();
    }
    /// <summary>
    /// Creates a mixmatch tag with values.
    /// </summary>
    /// <param name="customTags"> A dictionary containing the values to add. </param>
    public MixMatchTag(Dictionary<string, string> customTags)
    {
        _customTags = customTags;
    }
    /// <summary>
    /// Adds a new element to the metadata <br/>
    /// This can also be accomplished by using the [] operator.
    /// </summary>
    /// <param name="key"> The new key to add. </param>
    /// <param name="value"> The new value to add. </param>
    public void Add(string key, string value)
    {
        _customTags[key] = value;
    }
    /// <summary>
    /// Removes an element from the metadata by key.
    /// </summary>
    /// <param name="key"> The element to remove, if it exists. </param>
    public void Remove(string key)
    {
        _customTags.Remove(key);
    }
    /// <summary>
    /// Ienumerable implementation. Do not call.
    /// </summary>
    /// <returns> A new IEnumerator to use for enumeration. </returns>
    public IEnumerator GetEnumerator()
    {
        return new MixMatchTag(_customTags);
    }
    /// <summary>
    /// Serializes the MetadataTag to an XObject
    /// </summary>
    /// <returns>An XObject representing this tag. </returns>
    public XElement Serialize()
    {
        var content = (from kvp in _customTags 
            let key = new XAttribute("key", kvp.Key)
            let value = new XAttribute("value", kvp.Value)
            select new XElement("MixMatchTag", key, value)).ToArray();
        return new XElement("MixMatchTags", content);
    }
    /// <summary>
    /// IEnumerable implementation. Do not call.
    /// </summary>
    /// <returns> A boolean if the enumerable is still ongoing. </returns>
    public bool MoveNext()
    {
        _enumPos++;
        return _enumPos < _customTags.Keys.Count;
    }
    /// <summary>
    /// IEnumerable implementation. Do not call.
    /// </summary>
    public void Reset()
    {
        _enumPos = -1;
    }
    /// <summary>
    /// The current object in an enumerable loop.
    /// </summary>
    public KeyValuePair<string, string> Current
    {
        get
        {
            if (_enumPos < 0 || _enumPos > _customTags.Keys.Count) throw new InvalidOperationException();
            var key = _customTags.Keys.ToArray()[_enumPos];
            return new KeyValuePair<string, string>(key, _customTags[key]);
        }
    }

    object IEnumerator.Current => Current;

    /// <summary>
    /// Accesses a metadata tag, or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="s">The key to use for accessing</param>
    /// <returns>A string of the object if accessing. </returns>
    public string this[string s] => _customTags[s];
    public static bool operator ==(MixMatchTag a, MixMatchTag b)
    {
        return a
            .Cast<KeyValuePair<string, string>>()
            .All(kvp => a[kvp.Key].Equals(b[kvp.Key]));
    }

    public static bool operator !=(MixMatchTag a, MixMatchTag b)
    {
        return !(a == b);
    }
}
