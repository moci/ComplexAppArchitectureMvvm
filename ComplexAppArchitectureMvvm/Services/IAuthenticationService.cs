using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Stores;
using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface IAuthenticationService
	{
		Task<bool> TryGetAccess(AccessType type);
	}

	public enum AccessType
	{
		ACTION_A,
		ACTION_B,
	}

	public class AuthenticationService : IAuthenticationService
	{
		private readonly ILoggedInAccountStore mLoggedInAccountStore;
		private readonly IConfigurationStore mActiveConfigurationStore;
		private readonly IInteractionService mInteractionService;

		public AuthenticationService(ILoggedInAccountStore loggedInAccountStore, IConfigurationStore activeConfigurationStore, InteractionService interactionService)
		{
			mLoggedInAccountStore = loggedInAccountStore ?? throw new ArgumentNullException(nameof(loggedInAccountStore));
			mActiveConfigurationStore = activeConfigurationStore ?? throw new ArgumentNullException(nameof(activeConfigurationStore));
			mInteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
		}

		public Task<bool> TryGetAccess(AccessType type)
		{
			var tcs = new TaskCompletionSource<bool>();

			Task.Run(async () =>
			{
				var currentAccount = mLoggedInAccountStore.Read();
				var currentCfg = mActiveConfigurationStore.Read();

				var requiresPw = currentAccount?.Permissions.IsStrictModeEnabled ?? false;
				switch (type)
				{
					case AccessType.ACTION_A:
						requiresPw |= currentCfg?.ProtectActionA ?? false;
						break;
					case AccessType.ACTION_B:
						requiresPw |= currentCfg?.ProtectActionB ?? false;
						break;
					default:
						requiresPw |= false;
						break;
				}

				var requestedPassword = Password.Empty;
				if (requiresPw)
				{
					requestedPassword = currentAccount?.Password;
					// Fallback to system password if there is no password but we do require one.
					requestedPassword = Password.IsNullOrEmpty(requestedPassword) ? currentCfg?.SystemPassword : requestedPassword;
				}

				var gotAccess = true;
				if (!Password.IsNullOrEmpty(requestedPassword))
				{
					using var enterPassword = new EnterPasswordViewModel(requestedPassword);
					gotAccess = await mInteractionService.Prompt(enterPassword);
				}

				tcs.SetResult(gotAccess);
			});

			return tcs.Task;
		}
	}
}
