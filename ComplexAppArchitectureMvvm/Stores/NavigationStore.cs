using ComplexAppArchitectureMvvm.Services;
using System;

namespace ComplexAppArchitectureMvvm.Stores
{
	public interface INavigationStore : IStoreReader<INavigationable>, IStoreWriter<INavigationable> { }

	public class NavigationStore : Store<INavigationable>, INavigationStore
	{
		public NavigationStore() : base(null) { }

		public override void Write(INavigationable value)
		{
			// Dispose old value
			if (Read() is IDisposable oldDisposable) oldDisposable.Dispose();
			base.Write(value);
		}
	}
}
