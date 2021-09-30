using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System.Net;
using System.Security;
using System.Windows.Controls;

namespace ComplexAppArchitectureMvvm.Views.Interactions
{
	public partial class LoginView : UserControl
	{
		public LoginView()
		{
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var securePw = (DataContext as LoginViewModel)?.ProvidedPassword ?? new SecureString();
			PasswordBox.Password = new NetworkCredential(string.Empty, securePw).Password;
		}

		private void PasswordChangedHandler(object sender, System.Windows.RoutedEventArgs e)
		{
			if (DataContext is not LoginViewModel model) return;
			model.ProvidedPassword = PasswordBox.SecurePassword;
		}
	}

	public class LoginDesignData : LoginViewModel
	{
		public LoginDesignData() : base(null, new[] { AccountId.Factory.Create("user.1"), AccountId.Factory.Create("user.2") })
		{ }
	}
}
