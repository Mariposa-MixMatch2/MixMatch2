using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Win32.SafeHandles;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Models.Metadata;

namespace MixMatch2.Tests.SerializationTests
{
    internal class Mp3MetadataSerializationTest : ITest
    {
        private readonly Mp3Metadata _testMetadata;
        private readonly XmlSerializer _formatter;

        public Mp3MetadataSerializationTest()
        {
            _testMetadata = new Mp3Metadata();
            _testMetadata.Tags.AddRange(new[]
            {
                new MetadataTag("testString", "myString"),
                new MetadataTag("testPath", "./myPath.mp3"),
                new MetadataTag("testNum", 123.45),
                new MetadataTag("myCustomTags", new MixMatchTag(new()
                {
                    { "mixMatchTag1", "myValue1" },
                    { "mixMatchTag2", "myValue2" }
                })),
                new MetadataTag("myNestedTag", new MetadataTag("myKey", "myValue"))
            });

            

            _formatter = new XmlSerializer(typeof(Mp3Metadata), new []
            {
                typeof(MetadataTag)
            });
        }
        public async Task<TestResult> StartTest()
        {
            try
            {
                await Task.Run(() =>
                {
                    var fs = new FileStream(@"C:\Users\Liam\Desktop\Mp3MetadataSerializeTest.xml", FileMode.Create);
                    _formatter.Serialize(fs, _testMetadata);
                    fs.Close();
                });
                return new TestResult(true, "Test Succeeded");
            }
            catch (Exception ex)
            {
                return new TestResult(false, "Test failed with error: " + ex.Message, ex.StackTrace ?? "");
            }
            

        }

        public override string ToString()
        {
            return "Mp3MetadataSerializationTest";
        }
    }
}
