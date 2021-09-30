using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Repositories;
using ComplexAppArchitectureMvvm.Services;
using ComplexAppArchitectureMvvm.Stores;
using ComplexAppArchitectureMvvm.ViewModels;
using ComplexAppArchitectureMvvm.Windows;
using System;
using System.Diagnostics;
using System.Mvvm;
using System.Threading;
using System.Windows;

namespace ComplexAppArchitectureMvvm
{
	public partial class App : Application
	{
		private readonly ExitService mExitService;
		private ExitType? mRequestedExitType;

		private readonly InMemoryAccountRepository mAccounts;
		private readonly LoggedInAccountStore mLoggedInAccountStore;
		private readonly ConfigurationStore mActiveConfigurationStore;
		private readonly PasswordValidatorService mPasswordValidatorService;

		private readonly NavigationStore mPrimaryNavigationStore;
		private readonly Display mPrimaryWindow;
		private readonly LoginService mPrimaryLoginService;
		private readonly InteractionService mPrimaryInteractionService;
		private readonly AuthenticationService mPrimaryAuthenticationService;

		public App()
		{
			#region UI Init
			// To bind the enabled state to commands, we must ensure that the change event is fired on the UI thread
			Command.RaiseChangedEventDispatcher = Dispatcher;

			// Interaction views can set the title on the hosting window, the following hooks can be used to finetune
			// the title and window selection behavior
			Views.Interactions.Window.TitleTransformerHook = title => title?.ToUpperInvariant();
			Views.Interactions.Window.TitleWindowPredicateHook = window => window is Dialog;
			#endregion
			#region Repo init
			mAccounts = new InMemoryAccountRepository();
			// The student is strict, which means that he has to enter a password for everything, but because he does not have a password we'll use the system password...
			mAccounts.Set(Account.Factory.Create(AccountId.Factory.Create("student"), AccountPermissions.Factory.Create(isStrictModeEnabled: true), Password.Empty));
			mAccounts.Set(Account.Factory.Create(AccountId.Factory.Create("employee"), AccountPermissions.Factory.Create(), Password.Factory.FromClearText("test123")));
			mAccounts.Set(Account.Factory.Create(AccountId.Factory.Create("manager"), AccountPermissions.Factory.Create(), Password.Factory.FromClearText("pw123")));
			#endregion
			#region Stores init
			// Everyone will need to provide a password for performing action B
			mActiveConfigurationStore = new ConfigurationStore(Configuration.Factory.Create(requireLogin: true, protectActionB: true));
			mLoggedInAccountStore = new LoggedInAccountStore();
			#endregion
			#region Other shared services init
			// All exit requests (from inside of the application or from the WPF framework itself) are served through this service
			mExitService = new ExitService();
			mExitService.ExitRequested += OnExitRequested;

			mPasswordValidatorService = new PasswordValidatorService(mAccounts);
			#endregion
			#region Primary objects init
			// To be able to spawn dialogs from the correct window we have to duplicate a few services
			mPrimaryNavigationStore = new NavigationStore();
			mPrimaryWindow = new Display(mExitService) { DataContext = mPrimaryNavigationStore };
			mPrimaryInteractionService = new InteractionService(i => PerformPrompt(i, mPrimaryWindow), i => PerformFork(i, mPrimaryWindow));
			mPrimaryAuthenticationService = new AuthenticationService(mLoggedInAccountStore, mActiveConfigurationStore, mPrimaryInteractionService);
			mPrimaryLoginService = new LoginService(() => mPrimaryInteractionService, mLoggedInAccountStore, mPasswordValidatorService, mAccounts);
			#endregion
		}

		public bool IsExiting { get; private set; }
		private void OnExitRequested(object sender, ExitRequestedEventArgs e)
		{
			if (IsExiting) return;
			IsExiting = true;

			mRequestedExitType = e.Type.GetValueOrDefault(mActiveConfigurationStore.Read().DefaultExitType);

			var exitNavigationService = CreateUnloadingNavigationService(mPrimaryNavigationStore);
			exitNavigationService.Navigate();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var primaryNavigationService = CreateLoadingViewModelNavigationService(mPrimaryNavigationStore, mPrimaryLoginService, mPrimaryAuthenticationService, mPrimaryInteractionService);
			primaryNavigationService.Navigate();

			MainWindow = mPrimaryWindow;
			mPrimaryWindow.Show();
			mPrimaryWindow.Title = "Primary";
		}

		protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			// Invoked on user log-off and shutdown

			base.OnSessionEnding(e);

			if (e.ReasonSessionEnding == ReasonSessionEnding.Shutdown)
			{
				e.Cancel = true;
				MessageBox.Show("Close application before shutdown.");
			}
		}
		private void DispatcherUnhandledException_Handler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
			e.Handled = true;
			mExitService.RequestExit(ExitType.RESTART_APPLICATION);
		}

		private INavigationService CreateLoadingViewModelNavigationService(NavigationStore store, ILoginService loginService, IAuthenticationService authenticationService, InteractionService interactionService)
		{
			var loadingCompletedNavigationServices = new CompositeNavigationService(
				CreateHubNavigationService(store, loginService, authenticationService, interactionService),
				new GenericNavigationService(PositionWindows)
			);

			return new CompositeNavigationService(
				new NavigationService<LoadingViewModel>(() => new LoadingViewModel(loadingCompletedNavigationServices, mActiveConfigurationStore, mLoggedInAccountStore), store),
				new GenericNavigationService(() => ((LoadingViewModel)store.Read()).Load())
			);
		}
		private INavigationService CreateHubNavigationService(NavigationStore store, ILoginService loginService, IAuthenticationService authenticationService, IInteractionService interactionService)
		{
			return new NavigationService<HubViewModel>(() => new HubViewModel(mExitService, loginService, mLoggedInAccountStore, () => CreateActionsViewModelNavigationService(store, loginService, authenticationService, interactionService)), store);
		}
		private INavigationService CreateUnloadingNavigationService(NavigationStore store)
		{
			return new CompositeNavigationService(
				new NavigationService<UnloadingViewModel>(() => new UnloadingViewModel(new GenericNavigationService(PerformExit)), store, isExit: true),
				new GenericNavigationService(() => ((UnloadingViewModel)store.Read()).Unload())
			);
		}
		private INavigationService CreateActionsViewModelNavigationService(NavigationStore store, ILoginService loginService, IAuthenticationService authenticationService, IInteractionService interactionService)
		{
			return new NavigationService<ActionsViewModel>(() => new ActionsViewModel(() => CreateHubNavigationService(store, loginService, authenticationService, interactionService), interactionService, authenticationService), store);
		}

		private void PositionWindows()
		{
			if (!OnUitThread())
			{
				Dispatcher.BeginInvoke((Action)(() => PositionWindows()));
				return;
			}

			var cfg = mActiveConfigurationStore.Read();
			switch (cfg.StartType)
			{
				case ApplicationStartType.MAXIMIZED:
				case ApplicationStartType.KIOSK:
					var isKiosk = cfg.StartType == ApplicationStartType.KIOSK;
					WindowService.Maximize(mPrimaryWindow, isKiosk);
					break;
			}
		}
		private void PerformExit()
		{
			if (!OnUitThread())
			{
				Dispatcher.BeginInvoke((Action)(() => PerformExit()));
				return;
			}

			switch (mRequestedExitType)
			{
				case ExitType.SYSTEM_SHUTDOWN:
					Process.Start("shutdown", "-S");
					break;
				case ExitType.RESTART_APPLICATION:
					var exePath = string.Empty;

					using (var process = Process.GetCurrentProcess())
					{
						exePath = process.MainModule.FileName;
					}

					Process.Start(exePath);
					break;
			}

			Current.Shutdown();
		}
		private void PerformPrompt(IInteraction interaction, Window owner)
		{
			if (!OnUitThread())
			{
				Dispatcher.BeginInvoke((Action)(() => PerformPrompt(interaction, owner)));
				return;
			}

			var dialog = new Dialog
			{
				Owner = owner,
				DataContext = interaction
			};
			dialog.ShowDialog();
		}
		private void PerformFork(IInteraction interaction, Window owner)
		{
			if (!OnUitThread())
			{
				Dispatcher.BeginInvoke((Action)(() => PerformFork(interaction, owner)));
				return;
			}

			var dialog = new Dialog
			{
				Owner = owner,
				DataContext = interaction
			};
			dialog.Show();
		}

		private static bool OnUitThread() => Thread.CurrentThread == Current.Dispatcher.Thread;
	}
}
