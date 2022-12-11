using MixMatch2.Shared.Views.Pages;

namespace MixMatch2;

public partial class AppShell : Shell
{
	public double FlyoutWidth { get; set; }
	public AppShell()
    {
        Shell.SetTabBarBackgroundColor(this, new Color(0, 0, 0, 0));
		InitializeComponent();
		Routing.RegisterRoute("home", typeof(HomePage));
		Routing.RegisterRoute("tests", typeof(TestsPage));
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void MenuItem_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}