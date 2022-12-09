using MixMatch2.Shared.Views.Pages;

namespace MixMatch2;

public partial class AppShell : Shell
{
	public AppShell()
    {
        Shell.SetTabBarBackgroundColor(this, new Color(0, 0, 0, 0));
		InitializeComponent();
		Routing.RegisterRoute("home", typeof(HomePage));
		Routing.RegisterRoute("tests", typeof(TestsPage));
    }
}