using System;

namespace ComplexAppArchitectureMvvm.Domain
{
	public class Account
	{
		public static class Factory
		{
			public static Account Create(AccountId id, AccountPermissions permissions, Password password)
			{
				return new Account
				{
					Id = id ?? throw new ArgumentNullException(nameof(id)),
					Permissions = permissions,
					Password = password ?? Password.Empty,
				};
			}
		}

		private Account() { }

		public AccountId Id { get; private set; }
		public AccountPermissions Permissions { get; private set; }
		public Password Password { get; private set; }
	}
}
