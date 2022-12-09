using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using MixMatch2.Shared.Interfaces;

namespace MixMatch2.Shared.Models.Metadata
{
    public class Mp3Metadata : IMetadata
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var tagNames = new List<string>();
            foreach (var tag in Tags)
            {
                
                info.AddValue(tag.Key, tag, typeof(object));
                tagNames.Add(tag.Key);
            }

            info.AddValue("cumulativeTagNames", tagNames, typeof(List<string>));
        }

        public Mp3Metadata(SerializationInfo info, StreamingContext context)
        {
            Tags = new List<MetadataTag>();
            var tagNames = info.GetValue("cumulativeTagNames", typeof(List<string>)) as List<string> ??
                           throw new NullReferenceException("Failed to deserialize metadata tag list: " + this);
            foreach (var tag in tagNames)
            {
                Tags.Add(info.GetValue(tag, typeof(MetadataTag)) as MetadataTag ??
                         throw new NullReferenceException("Failed to deserialize metadata tag: " + tag));
            }
        }

        public List<MetadataTag> Tags { get; set; }

        public void WriteMetaToFile(FileResult file)
        {
            throw new NotImplementedException();
        }

        public Mp3Metadata()
        {
            Tags = new List<MetadataTag>();
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
