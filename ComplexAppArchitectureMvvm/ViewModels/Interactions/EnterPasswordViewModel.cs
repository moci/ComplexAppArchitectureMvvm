using ComplexAppArchitectureMvvm.Domain;
using System.Mvvm;
using System.Security;

namespace ComplexAppArchitectureMvvm.ViewModels.Interactions
{
	public class EnterPasswordViewModel : InteractionViewModelBase
	{
		private readonly Password mCorrectPassword;
		private SecureString mProvidedPassword = new();

		public EnterPasswordViewModel(Password correctPassword)
		{
			mCorrectPassword = Password.IsNullOrEmpty(correctPassword) ? Password.Empty : correctPassword;

			OkCmd = Command.Factory.Create(() =>
			{
				var result = mCorrectPassword == Password.Factory.FromSecureString(ProvidedPassword);
				MarkCompleted(result);
			});
		}

		public Command OkCmd { get; }

		public SecureString ProvidedPassword
		{
			get => mProvidedPassword;
			set => SetProperty(ref mProvidedPassword, value ?? new SecureString());
		}
	}
}
