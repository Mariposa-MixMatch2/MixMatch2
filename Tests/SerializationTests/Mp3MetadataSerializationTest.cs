using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32.SafeHandles;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Models.Metadata;

namespace MixMatch2.Tests.SerializationTests
{
    internal class Mp3MetadataSerializationTest : ITest
    {
        private readonly Metadata _testMetadata;

        /// <summary>
        /// Creates a new Mp3MetadataSerializationTest
        /// </summary>
        public Mp3MetadataSerializationTest()
        {
            _testMetadata = new Metadata()
            {
                {
                    "testString", 
                    "myString"
                },
                {
                    "testNum", 
                    123.45
                },
                {
                    "customTags", 
                    new MixMatchTag
                    {
                        { "mixMatchTag1", "myValue1" },
                        { "mixMatchTag2", "myValue2" }
                    }
                },
                {
                    "testNested",
                    new Metadata
                    {
                        {
                            "innerTestString",
                            "innerStringValue"
                        },
                        {
                            "innerTestNum",
                            678.90
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Begins this test asynchronously. 
        /// </summary>
        /// <returns>An awaitable TestResult. </returns>
        public async Task<TestResult> StartTest()
        {
            try
            {
                await Task.Run(() =>
                {
                    const string path = @"C:\Users\Liam\Desktop\Mp3MetadataSerializeTest.xml";
                    var stream = File.OpenWrite(path);
                    var xml = _testMetadata.Serialize();
                    xml.Save(stream);
                    stream.Close();
                });
                return new TestResult(true, "Test Succeeded");
            }
            catch (Exception ex)
            {
                return new TestResult(false, "Test failed with error: " + ex.Message, ex.StackTrace ?? "");
            }
            

        }
        /// <summary>
        /// Returns the name of this test.
        /// </summary>
        /// <returns> The name of this test. </returns>
        public override string ToString()
        {
            return "Mp3MetadataSerializationTest";
        }
    }
}
