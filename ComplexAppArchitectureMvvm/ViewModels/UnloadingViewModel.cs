using ComplexAppArchitectureMvvm.Services;
using System;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.ViewModels
{
	public class UnloadingViewModel : ViewModelBase, INavigationable
	{
		private readonly INavigationService mUnloadingCompletedNavigationService;

		private readonly object mUnloadLock = new object();
		private Task mInternalUnloadingTask;

		public UnloadingViewModel(INavigationService unloadingCompletedNavigationService)
		{
			mUnloadingCompletedNavigationService = unloadingCompletedNavigationService ?? throw new ArgumentNullException();
		}

		public bool IsUnloading => mInternalUnloadingTask != null;

		public void HandlePendingNavigation(PendingNavigationInfo info) { }

		public Task Unload()
		{
			lock (mUnloadLock)
			{
				if (mInternalUnloadingTask != null) return mInternalUnloadingTask;

				var tsc = new TaskCompletionSource();

				mInternalUnloadingTask = Task.Run(() =>
				{
					UnloadImplementation();
					tsc.SetResult();
				}).ContinueWith(async _ =>
				{
					mInternalUnloadingTask = null;
					RaisePropertyChangedEvent(nameof(IsUnloading));

					await mUnloadingCompletedNavigationService?.Navigate();
				});

				RaisePropertyChangedEvent(nameof(IsUnloading));

				return tsc.Task;
			}
		}

		private void UnloadImplementation()
		{
			Task.Delay(2000).Wait();
		}
	}
}
