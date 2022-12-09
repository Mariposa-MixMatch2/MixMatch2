using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MixMatch2.Shared.Interfaces;
using MixMatch2.Shared.Views.Tests;
using MixMatch2.Tests.SerializationTests;
using SkiaSharp;

namespace MixMatch2.Shared.ViewModels
{
    class TestsViewModel : ViewModelBase
    {
        #region Tests
        #region SerializationTests
        public ITest[] SerializationTests { get; set; } =
        {
            TestReference.GetTest("SerializationTests/Mp3MetadataSerializationTest")
        };
        public ICommand SerializationTestsExecuted { get; set; }
        #endregion
        #endregion

        public ISeries[] TestSuccessSeries { get; set; }
        public Axis[] YAxis { get; set; } =
        {
            new Axis()
            {
                ShowSeparatorLines = false
            }
        };

        private ObservableCollection<ObservableValue> _testsTotal;
        private ObservableCollection<ObservableValue> _testsSuccesses;

        public TestsViewModel()
        {
            var initVals = GetTestTotal(new[]
            {
                SerializationTests
            });

            _testsTotal = initVals[0];
            _testsSuccesses = initVals[1];

            TestSuccessSeries = new ISeries[]
            {
                new ColumnSeries<ObservableValue>
                {
                    IsHoverable = false,
                    Values = _testsTotal,
                    Stroke = null,
                    Fill = new SolidColorPaint(new SKColor(200, 0, 0, 100)),
                    IgnoresBarPosition = true
                },
                new ColumnSeries<ObservableValue>
                {
                    IsHoverable = false,
                    Values = _testsSuccesses,
                    Stroke = null,
                    Fill = new SolidColorPaint(new SKColor(0, 200, 0)),
                    IgnoresBarPosition = true
                }
            };
            SerializationTestsExecuted = new Command(
                execute: (res) =>
                {
                    var results = (TestResult[])res;
                    for (var i = 0; i < results.Length; i++)
                    {

                    }
                },
                canExecute: (discard) => true);
        }

        private static ObservableCollection<ObservableValue>[] GetTestTotal(ITest[][] tests)
        {
            var ret = new ObservableCollection<ObservableValue>();
            var ret2 = new ObservableCollection<ObservableValue>();
            foreach (var test in tests)
            {
                ret.Add(new ObservableValue(test.Length));
                ret2.Add(new ObservableValue(0));

            }

            return new [] {ret, ret2};
        }
    }
    internal static class TestReference
    {
        private static readonly Dictionary<string, object> Tests = new()
        {
            {
                "SerializationTests", new Dictionary<string, object>
                {
                    {
                        
                        "Mp3MetadataSerializationTest", 
                        new Mp3MetadataSerializationTest()
                    }
                }
            }
        };

        public static ITest GetTest(string testPath)
        {
            var path = testPath.Split('/');
            var current = Tests;
            for (var i = 0; i < path.Length - 1; i++)
            {
                if (Tests[path[i]].GetType() != typeof(Dictionary<string, object>))
                    throw new ArgumentException("Invalid path: " + testPath);
                current = (Dictionary<string, object>)current[path[i]];
            }
            if (current[path[^1]] is not ITest)
                throw new ArgumentException("Invalid path: " + testPath);
            return (ITest)current[path[^1]];
        }

    }
}
