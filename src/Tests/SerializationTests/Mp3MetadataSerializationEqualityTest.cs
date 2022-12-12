using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Models.Metadata;

namespace MixMatch2.Tests.SerializationTests
{
    internal class Mp3MetadataSerializationEqualityTest : ITest
	{
		private Metadata _testMetadata;

		/// <summary>
		/// Creates a new Mp3MetadataSerializatoinEqualityTest
		/// </summary>
		public Mp3MetadataSerializationEqualityTest()
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
		/// Runs the Mp3MetadataSerializationEqualityTest
		/// </summary>
		/// <returns>
		/// A TestResult detailing the results of the test.
		/// </returns>
		public async Task<TestResult> StartTest()
		{
			try
			{
				var newMeta = new Metadata();
				await Task.Run(() =>
				{
					var xml = _testMetadata.Serialize();
					newMeta = Metadata.Deserialize(xml);
				});
				return new TestResult(_testMetadata == newMeta, "Success");
			}
			catch (Exception ex)
			{
				return new TestResult(false, ex.Message, ex.StackTrace);
			}
		}
		/// <summary>
		/// Returns the name of this test.
		/// </summary>
		/// <returns> The name of this test. </returns>
		public override string ToString()
		{
			return "Mp3MetadataSerializationEqualityTest";
		}
	}
}
