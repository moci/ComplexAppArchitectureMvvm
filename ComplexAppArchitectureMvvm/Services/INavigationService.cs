using ComplexAppArchitectureMvvm.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface INavigationService
	{
		Task<bool> Navigate();
	}

	public interface INavigationable
	{
		void HandlePendingNavigation(PendingNavigationInfo info);
	}

	public class PendingNavigationInfo
	{
		public PendingNavigationInfo(bool isExit, Type targetType)
		{
			TargetType = targetType;
			IsExit = isExit;
		}

		public Type TargetType { get; }
		public bool IsCancelled { get; private set; }
		public bool IsExit { get; }

		/// <summary>
		/// Try to cancel the pending navigation
		/// </summary>
		public bool TryCancel()
		{
			if (IsExit) return false;

			IsCancelled = true;
			return true;
		}
	}

	public class NavigationService<TTargetType> : INavigationService where TTargetType : INavigationable
	{
		private readonly NavigationStore mStore;
		private readonly Func<TTargetType> mTargetProvider;
		private readonly bool mIsExit;

		public NavigationService(Func<TTargetType> targetProvider, NavigationStore store, bool isExit = false)
		{
			mIsExit = isExit;
			mTargetProvider = targetProvider;
			mStore = store ?? throw new ArgumentNullException(nameof(store));
		}

		public Task<bool> Navigate()
		{
			var tsc = new TaskCompletionSource<bool>();

			Task.Run(() =>
			{
				var pendingNavigationInfo = new PendingNavigationInfo(mIsExit, typeof(TTargetType));

				var previous = mStore.Read();
				previous?.HandlePendingNavigation(pendingNavigationInfo);

				if (!pendingNavigationInfo.IsCancelled) mStore.Write(mTargetProvider.Invoke());

				tsc.SetResult(!pendingNavigationInfo.IsCancelled);
			});

			return tsc.Task;
		}
	}
	public class CompositeNavigationService : INavigationService
	{
		private readonly IEnumerable<INavigationService> mServices;

		public CompositeNavigationService(params INavigationService[] services)
		{
			mServices = services;
		}
		public CompositeNavigationService(IEnumerable<INavigationService> services)
		{
			mServices = services ?? Array.Empty<INavigationService>();
		}

		public Task<bool> Navigate()
		{
			var tsc = new TaskCompletionSource<bool>();

			Task.Run(async () =>
			{
				var navigationStopped = false;
				foreach (var service in mServices)
				{
					if (!await service.Navigate())
					{
						navigationStopped = true;
						break;
					}
				}

				tsc.SetResult(!navigationStopped);
			});

			return tsc.Task;
		}
	}
	public class GenericNavigationService : INavigationService
	{
		private readonly Action mAction;

		public GenericNavigationService(Action action)
		{
			mAction = action;
		}

		public Task<bool> Navigate()
		{
			var tsc = new TaskCompletionSource<bool>();

			Task.Run(() =>
			{
				mAction?.Invoke();
				tsc.SetResult(true);
			});

			return tsc.Task;
		}
	}
}
