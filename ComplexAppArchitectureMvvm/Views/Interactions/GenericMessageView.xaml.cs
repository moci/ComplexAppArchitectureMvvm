using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System.Windows.Controls;
using static ComplexAppArchitectureMvvm.ViewModels.Interactions.InputCommand;

namespace ComplexAppArchitectureMvvm.Views.Interactions
{
	public partial class GenericMessageView : UserControl
	{
		public GenericMessageView()
		{
			InitializeComponent();
		}
	}

	public class GenericMessageDesignData : GenericMessageViewModel
	{
		public GenericMessageDesignData() : base(MessageType.COMPLETED, new[] { InputType.YES, InputType.NO }) { }
	}
}
