using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Services;
using ComplexAppArchitectureMvvm.Stores;
using System;
using System.Mvvm;

namespace ComplexAppArchitectureMvvm.ViewModels
{
	public class HubViewModel : ViewModelBase, INavigationable
	{
		private readonly ILoggedInAccountStore mLoggedInAccountStore;
		private readonly ILoginService mLoginService;

		public HubViewModel(IExitService exitService, ILoginService loginService, ILoggedInAccountStore loggedInAccountStore, Func<INavigationService> toActionServiceProvider)
		{
			mLoginService = loginService ?? throw new ArgumentNullException(nameof(loginService));

			mLoggedInAccountStore = loggedInAccountStore ?? throw new ArgumentNullException(nameof(loggedInAccountStore));
			mLoggedInAccountStore.ValueChanged += OnLoggedInAccountChanged;

			ExitCmd = Command.Factory.Create(() => exitService.RequestExit());
			LoginCmd = Command.Factory.Create(() => mLoginService.Login());
			LogoutCmd = Command.Factory.Create(() => loginService.Logout(), () => mLoggedInAccountStore.IsLoggedIn);
			ToActionsCmd = Command.Factory.Create(() => toActionServiceProvider().Navigate());
		}
		public Account LoggedInAccount => mLoggedInAccountStore?.Read();
		public Command ExitCmd { get; }
		public Command LoginCmd { get; }
		public Command LogoutCmd { get; }
		public Command ToActionsCmd { get; }

		public void HandlePendingNavigation(PendingNavigationInfo info) { }
		private void OnLoggedInAccountChanged(object sender, EventArgs e)
		{
			RaisePropertyChangedEvent(nameof(LoggedInAccount));
			LogoutCmd.RaiseCanExecuteChanged();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (mLoggedInAccountStore != null) mLoggedInAccountStore.ValueChanged -= OnLoggedInAccountChanged;
		}
	}
}
