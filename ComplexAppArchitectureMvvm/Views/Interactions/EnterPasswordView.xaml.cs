using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace ComplexAppArchitectureMvvm.Views.Interactions
{
	public partial class EnterPasswordView : UserControl
	{
		public EnterPasswordView()
		{
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var securePw = (DataContext as EnterPasswordViewModel)?.ProvidedPassword ?? new SecureString();
			PasswordBox.Password = new NetworkCredential(string.Empty, securePw).Password;
		}

		private void PasswordChangedHandler(object sender, RoutedEventArgs e)
		{
			if (DataContext is not EnterPasswordViewModel model) return;
			model.ProvidedPassword = PasswordBox.SecurePassword;
		}
	}

	public class EnterPasswordDesignData : EnterPasswordViewModel
	{
		public EnterPasswordDesignData() : base(Password.Factory.FromClearText("pwd123"))
		{ }
	}
}
