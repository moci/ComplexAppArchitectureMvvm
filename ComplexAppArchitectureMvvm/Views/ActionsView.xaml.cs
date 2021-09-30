using ComplexAppArchitectureMvvm.ViewModels;
using System.Windows.Controls;

namespace ComplexAppArchitectureMvvm.Views
{
	public partial class ActionsView : UserControl
	{
		public ActionsView()
		{
			InitializeComponent();
		}
	}

	public class ActionsDesignData : ActionsViewModel
	{
		public ActionsDesignData() : base(null, null, null)
		{ }
	}
}
