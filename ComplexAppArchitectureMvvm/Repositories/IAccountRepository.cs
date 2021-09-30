using ComplexAppArchitectureMvvm.Domain;
using System;
using System.Collections.Generic;

namespace ComplexAppArchitectureMvvm.Repositories
{
	public interface IAccountRepository : IRepository<Account, AccountId>
	{
		IEnumerable<AccountId> GetAccountIds();
	}

	public class InMemoryAccountRepository : IAccountRepository
	{
		private readonly Dictionary<AccountId, Account> mAccounts;

		public InMemoryAccountRepository()
		{
			mAccounts = new();
		}

		public IEnumerable<AccountId> GetAccountIds() => mAccounts.Keys;
		public IEnumerable<Account> GetAll() => mAccounts.Values;
		public Account Get(AccountId id)
		{
			id = AccountId.IsNullOrEmpty(id) ? AccountId.Empty : id;
			return mAccounts.TryGetValue(id, out var account) ? account : null;
		}
		public void Remove(AccountId id)
		{
			if (AccountId.IsNullOrEmpty(id)) return;
			mAccounts.Remove(id);
		}
		public void Set(Account value)
		{
			if (AccountId.IsNullOrEmpty(value?.Id)) throw new ArgumentException("Account must be valid and/or have a valid account id");
			mAccounts[value.Id] = value;
		}
	}
}
