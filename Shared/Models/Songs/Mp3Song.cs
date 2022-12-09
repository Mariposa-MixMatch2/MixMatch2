using System.Runtime.Serialization;
using MixMatch2.Shared.Interfaces;

namespace MixMatch2.Shared.Classes.Songs;

[Serializable]
public class Mp3Song : ISong, ISerializable
{
    public IMetadata Metadata { get; set; }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }
}