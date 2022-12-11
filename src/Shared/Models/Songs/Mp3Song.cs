using System.Runtime.Serialization;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Models.Metadata;

namespace MixMatch2.Shared.Classes.Songs;

[Serializable]
public class Mp3Song : ISong
{
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }
}