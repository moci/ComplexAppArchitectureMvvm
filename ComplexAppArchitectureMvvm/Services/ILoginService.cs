using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Repositories;
using ComplexAppArchitectureMvvm.Stores;
using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface ILoginService
	{
		void Logout();
		Task<Account> Login();
	}

	public class LoginService : ILoginService
	{
		private readonly IAccountStore mLoggedInAccountStore;
		private readonly Func<IInteractionService> mInteractionServiceProvider;
		private readonly IPasswordValidatorService mPasswordValidator;
		private readonly IAccountRepository mAccountProvider;

		public LoginService(
			Func<IInteractionService> interactionServiceProvider,
			IAccountStore loggedInAccountStore,
			IPasswordValidatorService passwordValidator,
			IAccountRepository accountProvider)
		{
			mLoggedInAccountStore = loggedInAccountStore ?? throw new ArgumentNullException(nameof(loggedInAccountStore));
			mInteractionServiceProvider = interactionServiceProvider ?? throw new ArgumentNullException(nameof(interactionServiceProvider));
			mPasswordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
			mAccountProvider = accountProvider ?? throw new ArgumentNullException(nameof(accountProvider));
		}

		public Task<Account> Login()
		{
			var tcs = new TaskCompletionSource<Account>();

			Task.Run(async () =>
			{
				using var model = new LoginViewModel(mPasswordValidator, mAccountProvider.GetAccountIds());

				if (await mInteractionServiceProvider().Prompt(model))
				{
					var account = mAccountProvider.Get(model.SelectedAccountId);
					mLoggedInAccountStore.Write(account);
				}

				tcs.SetResult(mLoggedInAccountStore.Read());
			});

			return tcs.Task;

		}

		public void Logout()
		{
			mLoggedInAccountStore.Write(null);
		}
	}
}
