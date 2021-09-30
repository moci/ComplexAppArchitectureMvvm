using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Services;
using ComplexAppArchitectureMvvm.Stores;
using System;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.ViewModels
{
	public class LoadingViewModel : ViewModelBase, INavigationable
	{
		private readonly INavigationService mLoadingCompletedNavigationService;
		private readonly IConfigurationStore mConfigurationStore;
		private readonly IAccountStore mAccountStore;

		private readonly object mLoadLock = new();
		private Task mInternalLoadingTask;

		public LoadingViewModel(INavigationService loadingCompleteNavigationService, IConfigurationStore configurationStore, IAccountStore accountStore)
		{
			mLoadingCompletedNavigationService = loadingCompleteNavigationService ?? throw new ArgumentNullException(nameof(loadingCompleteNavigationService));
			mConfigurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
			mAccountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
		}

		public bool IsLoading => mInternalLoadingTask != null;

		public void HandlePendingNavigation(PendingNavigationInfo info) { }

		public Task Load()
		{
			lock (mLoadLock)
			{
				if (mInternalLoadingTask != null) return mInternalLoadingTask;

				var tsc = new TaskCompletionSource();

				mInternalLoadingTask = Task.Run(() =>
				{
					LoadImplementation();
					tsc.SetResult();
				}).ContinueWith(async _ =>
				{
					mInternalLoadingTask = null;
					RaisePropertyChangedEvent(nameof(IsLoading));

					await mLoadingCompletedNavigationService?.Navigate();
				});

				RaisePropertyChangedEvent(nameof(IsLoading));

				return tsc.Task;
			}
		}

		private void LoadImplementation()
		{
			Task.Delay(2000).Wait();

			// Load config from some place...
			var config = Configuration.Factory.Create(
				systemPassword: Password.Factory.FromClearText("loaded password"),
				protectActionB: true,
				defaultExitType: ExitType.EXIT_TO_DESKTOP);
			mConfigurationStore.Write(config);

			if (config.RequireLogin)
			{
				// Perform logon and set logged in account
				mAccountStore.Write(null);
			}
		}



	}
}
