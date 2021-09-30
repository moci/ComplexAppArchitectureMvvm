using ComplexAppArchitectureMvvm.Domain;

namespace ComplexAppArchitectureMvvm.Stores
{
	public interface IAccountStore : IStoreReader<Account>, IStoreWriter<Account>
	{ }
	public class AccountStore : Store<Account>, IAccountStore
	{
		public AccountStore() : base(null) { }
	}

	public interface ILoggedInAccountStore : IAccountStore
	{
		bool IsLoggedIn { get; }
	}

	public class LoggedInAccountStore : AccountStore, ILoggedInAccountStore
	{
		public bool IsLoggedIn => !AccountId.IsNullOrEmpty(Read()?.Id);
	}
}
