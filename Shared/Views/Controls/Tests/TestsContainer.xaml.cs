using System.Windows.Input;
using MixMatch2.Resources.Helpers;
using MixMatch2.Shared.Interfaces;

namespace MixMatch2.Shared.Views.Tests;

public partial class TestsContainer : ContentView
{
    public static readonly BindableProperty TestExecutedProperty =
        BindableProperty.Create(nameof(TestExecuted), typeof(ICommand), typeof(TestsContainer));

    public static BindableProperty TestsProperty = null!;

    public ITest[] Tests
    {
        get => (ITest[])GetValue(TestsProperty);
        set
        {
            SetValue(TestsProperty, value);
            
        }
    }

    public ICommand TestExecuted
    {
        get => (ICommand)GetValue(TestExecutedProperty);
        set => SetValue(TestExecutedProperty, value);
    }
    private VisualElement[] _individualTests;

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
        InitializeComponent();
    }

    private VisualElement BuildSingleTest(int index)
    {
        var singleTest = new HorizontalStackLayout();
        var runTest = new Button();
        var text = new Label();

        runTest.Clicked += RunSingleTestOnClick;
        runTest.CommandParameter = index.ToString();
        runTest.WidthRequest = 30;
        runTest.HeightRequest = 30;
        runTest.BackgroundColor = Colors.Transparent;
        runTest.BorderColor = Colors.Transparent;
        runTest.FontFamily = "MaterialDesignIcons";
        runTest.Text = MaterialDesignIconsFonts.Play;
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

    private async void RunSingleTestOnClick(object? sender, EventArgs e)
    {
        if (!TestExecuted.CanExecute(null)) return;

        var results = new TestResult[_individualTests.Length];

        if (sender == null) return;
        var button = (Button)sender;
        var ind = int.Parse((string)button.CommandParameter);
        results[ind] = await Tests[ind].StartTest();
        _individualTests[ind].BackgroundColor = 
            results[ind].Success ? 
                new Color(0, 200, 0, 20) : 
                new Color(200,0, 0, 20);

        TestExecuted.Execute(results);
    }

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
                completed.Result.Success ?
                    new Color(0, 200, 0, 20) :
                    new Color(200, 0, 0, 20);
            tasks.Remove(completed);
        }

        TestExecuted.Execute(results);

        if (results.All(x => x.Success))
        {
            //green
        }
        else if (results.Any(x => x.Success))
        {
            //orange
        }
        else
        {
            //red
        }
    }
}