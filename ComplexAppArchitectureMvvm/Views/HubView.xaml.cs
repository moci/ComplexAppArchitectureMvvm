using ComplexAppArchitectureMvvm.Domain;
using ComplexAppArchitectureMvvm.Stores;
using ComplexAppArchitectureMvvm.ViewModels;
using System;
using System.Windows.Controls;

namespace ComplexAppArchitectureMvvm.Views
{
	public partial class HubView : UserControl
	{
		public HubView()
		{
			InitializeComponent();
		}
	}

	public class HubDesignData : HubViewModel
	{
		public HubDesignData() : base(null, null, new DummyLoggedInAccountStore(), null) { }

		public class DummyLoggedInAccountStore : ILoggedInAccountStore
		{
			public event EventHandler ValueChanged = delegate { };
			public bool IsLoggedIn => true;

			public Account Read() => Account.Factory.Create(AccountId.Factory.Create("dummy.acc"), AccountPermissions.Factory.Create(isStrictModeEnabled: true), Password.Empty);
			public void Write(Account value)
			{
				ValueChanged(this, EventArgs.Empty);
			}
		}
	}
}
