namespace ComplexAppArchitectureMvvm.Domain
{
	public class AccountPermissions
	{
		public static class Factory
		{
			public static AccountPermissions Create(bool? isStrictModeEnabled = null)
			{
				return new AccountPermissions
				{
					IsStrictModeEnabled = isStrictModeEnabled.GetValueOrDefault(false),
				};
			}
		}

		private AccountPermissions() { }

		/// <summary>
		/// When strict mode is enabled, a password is required for most actions
		/// </summary>
		public bool IsStrictModeEnabled { get; private set; }
	}
}
