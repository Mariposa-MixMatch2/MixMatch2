using System.Windows.Input;
using LiveChartsCore.Kernel;
using MixMatch2.Resources.Helpers;
using MixMatch2.Shared.Interfaces;
using Range = MixMatch2.Resources.Helpers.Range;

namespace MixMatch2.Shared.Views.Tests;

public partial class TestsContainer : ContentView
{
    public static readonly BindableProperty TestExecutedProperty =
        BindableProperty.Create(nameof(TestExecuted), typeof(ICommand), typeof(TestsContainer));

    public static BindableProperty TestsProperty = null!;
    public static BindableProperty NameProperty = null!;

    /// <summary>
    /// An array of all tests that should be contained within this TestContainer.
    /// </summary>
    public ITest[] Tests
    {
        get => (ITest[])GetValue(TestsProperty);
        set => SetValue(TestsProperty, value);
    }

    /// <summary>
    /// The name of this TestsContainer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A callback that is called after tests are executed.
    /// Contains an array of TestResults, where null means the test was not executed.
    /// </summary>
    public ICommand TestExecuted
    {
        get => (ICommand)GetValue(TestExecutedProperty);
        set => SetValue(TestExecutedProperty, value);
    }

    private VisualElement[] _individualTests;

    /// <summary>
    /// The TestsContainer Constructor. This shouldn't be called manually, but rather by the xaml.
    /// </summary>
    public TestsContainer()
    {
        _individualTests = Array.Empty<VisualElement>();
        TestsProperty = BindableProperty.Create(
            nameof(Tests),
            typeof(ITest[]),
            typeof(TestsContainer),
            propertyChanged: (sender, oldVal, newVal) =>
            {
                _individualTests = new VisualElement[((ITest[])newVal).Length];
                for (var i = 0; i < ((ITest[])newVal).Length; i++)
                {
                    _individualTests[i] = BuildSingleTest(i);
                    IndividualTests.Children.Add(_individualTests[i]);
                }
            });
        NameProperty = BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(TestsContainer),
            propertyChanged: (sender, oldVal, newVal) => { ContainerLabel.Text = (string)newVal; });
        InitializeComponent();
    }

    /// <summary>
    /// Builds a VisualElement representing a single unit test
    /// </summary>
    /// <param name="index"> The index in Tests that this visual element will build</param>
    /// <returns> A single row in the expanded TestContainer </returns>
    private VisualElement BuildSingleTest(int index)
    {
        //todo: make 'text' have an underlining bar that changes color rather than the whole background
        var singleTest = new HorizontalStackLayout();
        var runTest = new Button();
        var text = new Label();

        runTest.Clicked += RunSingleTestOnClick;
        runTest.CommandParameter = index.ToString();
        runTest.WidthRequest = 50;
        runTest.HeightRequest = 50;
        runTest.BackgroundColor = Colors.Transparent;
        runTest.BorderColor = Colors.Transparent;
        runTest.FontFamily = "MaterialDesignIcons";
        runTest.FontSize = 20;
        runTest.Text = MaterialDesignIconsFonts.Play;
        text.VerticalTextAlignment = TextAlignment.Center;
        text.VerticalOptions = LayoutOptions.Center;
        text.Text = Tests[index].ToString();
        text.WidthRequest = 200;
        text.HeightRequest = 30;
        text.VerticalTextAlignment = TextAlignment.Center;
        text.VerticalOptions = LayoutOptions.Center;

        singleTest.Add(runTest);
        singleTest.Add(text);
        singleTest.HeightRequest = 30;

        return singleTest;
    }

    /// <summary>
    /// Runs a single unit test
    /// </summary>
    /// <param name="sender"> The Button corresponding to the unit test </param>
    /// <param name="e"> ButtonClick Event Args </param>
    private async void RunSingleTestOnClick(object? sender, EventArgs e)
    {
        //todo: code is duplicated here and in RunAllTests, make helper function?
        if (!TestExecuted.CanExecute(null)) return;

        var results = new TestResult[_individualTests.Length];

        if (sender == null) return;
        var button = (Button)sender;
        var ind = int.Parse((string)button.CommandParameter);
        results[ind] = await Tests[ind].StartTest();
        _individualTests[ind].BackgroundColor =
            results[ind].Success ? new Color(0, 200, 0, 20) : new Color(200, 0, 0, 20);

        TestExecuted.Execute(results);
    }

    /// <summary>
    /// Runs all unit test
    /// </summary>
    /// <param name="sender"> The RunAllUnitTests button </param>
    /// <param name="e"> ButtonClick Event Args</param>
    private async void RunAllTestsOnClick(object? sender, EventArgs e)
    {
        if (!TestExecuted.CanExecute(null)) return;

        var tasks =
            new List<Task<TestResult>>(Tests.Select(x => x.StartTest()).ToArray());
        var index = new List<Task<TestResult>>(tasks);
        var results = new TestResult[_individualTests.Length];

        while (tasks.Any())
        {
            var completed = await Task.WhenAny(tasks).ConfigureAwait(false);
            var ind = index.IndexOf(completed);

            results[ind] = completed.Result;
            _individualTests[ind].BackgroundColor =
                completed.Result.Success ? new Color(0, 200, 0, 20) : new Color(200, 0, 0, 20);
            tasks.Remove(completed);
        }

        TestExecuted.Execute(results);

        if (results.All(x => x.Success))
        {
            _primaryColor = (Color)Resources["TestSuccess"];
        }
        else if (results.Any(x => x.Success))
        {
            _primaryColor = (Color)Resources["TestPartial"];
        }
        else
        {
            _primaryColor = (Color)Resources["TestFailed"];
        }
        //todo: animate the color change (clever gradient stop usage perhaps?)
    }


    private bool _openState = false;
    private Color? _primaryColor = null;
    /// <summary>
    /// Handles the animations that occur when the Test Container is expanded.
    /// </summary>
    /// <param name="sender"> ExpandTestContainer button </param>
    /// <param name="e"> ButtonClick Event args </param>
    private void ExpandTestContainer_OnClicked(object? sender, EventArgs e)
    {
        var startX = 0.0;
        var startY = 0.0;
        var endX = 0.0;
        var endY = 0.0;
        var rotation = 0.0;
        var height = 0.0;
        _primaryColor ??= (Color)Resources["TestNotRan"];
        var slate = (Color)Application.Current.Resources.MergedDictionaries.First()["Slate"];
        if (_openState)
        {
            startX = 1;
            startY = 0;
            endX = 0;
            endY = 1;
            rotation = 180;
            height = 50 * (_individualTests.Length + 1);
            var anim = new Animation(
                (progress) =>
                {
                    if (progress <= 0.5)
                    {
                        endX = progress * 2;
                        startX = 1;
                    }
                    else
                    {
                        IndividualTests.IsVisible = false;
                        endX = 1;
                        startX = 1 - (progress - 0.5) * 2;
                    }
                    rotation = 180 + 180 * progress;
                    height = MathHelper.MapRange(new Range(0, 1),
                        new Range(50 * _individualTests.Length, 0), progress);

                    var stroke = new LinearGradientBrush(
                        new GradientStopCollection
                        {
                            new GradientStop(_primaryColor, 0),
                            new GradientStop(slate, 1)
                        },
                        new Point(startX, startY),
                        new Point(endX, endY));
                    OuterBorder.Stroke = stroke;

                    ExpandTestContainer.Rotation = rotation;
                    OuterGrid.RowDefinitions[1].Height = height;
                });
            anim.Commit(this, "Close", 16, 200, Easing.SinInOut, (d, b) =>
            {
                _openState = false;
                
            });
        }
        else
        {
            startX = 0;
            startY = 1;
            endX = 1;
            endY = 0;
            rotation = 0;
            height = 50;
            var anim = new Animation(
                (progress) =>
                {
                    if (progress <= 0.5)
                    {
                        endX = 1 - progress * 2;
                        startX = 0;
                    }
                    else
                    {
                        IndividualTests.IsVisible = true;
                        endX = 0;
                        startX = (progress - 0.5) * 2;
                    }
                    rotation = 180 * progress;
                    height = MathHelper.MapRange(new Range(0, 1),
                        new Range(0, 50 * _individualTests.Length), progress);

                    var stroke = new LinearGradientBrush(
                        new GradientStopCollection
                        {
                            new GradientStop(_primaryColor, 0),
                            new GradientStop(slate, 1)
                        },
                        new Point(startX, startY),
                        new Point(endX, endY));
                    OuterBorder.Stroke = stroke;

                    ExpandTestContainer.Rotation = rotation;
                    OuterGrid.RowDefinitions[1].Height = height;
                });
            anim.Commit(this, "Close", 16, 200, Easing.SinInOut, (d, b) =>
            {
                _openState = true;
            });
        }
    }
}