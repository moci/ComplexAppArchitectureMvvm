namespace ComplexAppArchitectureMvvm.Domain
{
	public class Configuration
	{
		public static class Factory
		{
			public static Configuration Create(
				ApplicationStartType? startType = null,
				Password systemPassword = null,
				bool? requireLogin = null,
				bool? protectActionA = null,
				bool? protectActionB = null,
				ExitType? defaultExitType = null)
			{
				return new Configuration
				{
					StartType = startType.GetValueOrDefault(ApplicationStartType.NORMAL),
					SystemPassword = systemPassword ?? Password.Factory.FromClearText("214016"),
					RequireLogin = requireLogin.GetValueOrDefault(false),
					DefaultExitType = defaultExitType.GetValueOrDefault(ExitType.SYSTEM_SHUTDOWN),
					ProtectActionA = protectActionA.GetValueOrDefault(false),
					ProtectActionB = protectActionB.GetValueOrDefault(false),
				};
			}
		}

		private Configuration() { }

		public ApplicationStartType StartType { get; private set; }
		public Password SystemPassword { get; private set; }
		public bool RequireLogin { get; private set; }
		public bool ProtectActionA { get; private set; }
		public bool ProtectActionB { get; private set; }
		public ExitType DefaultExitType { get; private set; }
	}

	public enum ExitType
	{
		EXIT_TO_DESKTOP,
		SYSTEM_SHUTDOWN,
		RESTART_APPLICATION
	}

	public enum ApplicationStartType
	{
		NORMAL,
		MAXIMIZED,
		KIOSK,
	}
}
