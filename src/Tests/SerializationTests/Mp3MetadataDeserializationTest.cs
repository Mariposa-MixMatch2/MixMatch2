using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Models.Metadata;

namespace MixMatch2.Tests.SerializationTests
{
    internal class Mp3MetadataDeserializationTest : ITest
	{
		private XElement _meta;
		/// <summary>
		/// Creates a new mp3 metadata deserialization test.
		/// </summary>
		public Mp3MetadataDeserializationTest()
		{
			_meta = new Metadata()
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
			}.Serialize();
		}
		/// <summary>
		/// Starts the deserialization test
		/// </summary>
		/// <returns>
		///	A TestResult detailing the results of the test.
		/// </returns>
		public async Task<TestResult>StartTest()
		{
			try
			{
				await Task.Run(() =>
				{
					var newMeta = Metadata.Deserialize(_meta);

				});
				return new TestResult(true, "Success");
			}
			catch (Exception ex)
			{
				return new TestResult(false, ex.Message, ex.StackTrace);
			}
		}
		/// <summary>
		/// Gets a string name of this test
		/// </summary>
		/// <returns> The name of this test. </returns>
		public override string ToString()
		{
			return "Mp3MetadataDeserializationTest";
		}
	}
}
