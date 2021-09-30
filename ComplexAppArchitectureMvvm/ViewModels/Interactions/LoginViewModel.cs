using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvvm;
using System.Security;

namespace ComplexAppArchitectureMvvm.ViewModels.Interactions
{
	public class LoginViewModel : InteractionViewModelBase
	{
		private AccountId mSelectedId;
		private SecureString mProvidedPassword = new();

		public LoginViewModel(IPasswordValidatorService passwordValidator, IEnumerable<AccountId> accounts)
		{
			OkCmd = Command.Factory.Create(() =>
			{
				var result = passwordValidator.IsValidPassword(SelectedAccountId, Password.Factory.FromSecureString(ProvidedPassword));
				MarkCompleted(result);
			});

			Accounts = (accounts ?? Array.Empty<AccountId>()).ToArray();
			SelectedAccountId = Accounts.FirstOrDefault();
		}

		public Command OkCmd { get; }

		public IEnumerable<AccountId> Accounts { get; }
		public AccountId SelectedAccountId
		{
			get => mSelectedId;
			set => SetProperty(ref mSelectedId, value).OnChanged(() => OkCmd.RaiseCanExecuteChanged());
		}
		public SecureString ProvidedPassword
		{
			get => mProvidedPassword;
			set => SetProperty(ref mProvidedPassword, value ?? new SecureString()).OnChanged(() => OkCmd.RaiseCanExecuteChanged());
		}
	}
}
