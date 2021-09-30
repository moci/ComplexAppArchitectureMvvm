using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Repositories;
using System;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface IPasswordValidatorService
	{
		bool IsValidPassword(AccountId id, Password password);
	}

	public class PasswordValidatorService : IPasswordValidatorService
	{
		private readonly IAccountRepository mAccounts;

		public PasswordValidatorService(IAccountRepository accounts)
		{
			mAccounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
		}

		public bool IsValidPassword(AccountId id, Password password)
		{
			var account = mAccounts.Get(id);
			return account?.Password == password;
		}
	}
}
