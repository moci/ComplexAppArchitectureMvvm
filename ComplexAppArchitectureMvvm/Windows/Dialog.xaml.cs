using ComplexAppArchitectureMvvm.Services;
using System.Threading.Tasks;
using System.Windows;

namespace ComplexAppArchitectureMvvm.Windows
{
	public partial class Dialog : Window
	{
		private bool mPassDialogResultCheckDuringClose = false;

		public Dialog()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Closing += OnClosing;
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DataContext is IInteraction interaction)
			{
				if (!mPassDialogResultCheckDuringClose && DialogResult == null)
				{
					e.Cancel = true;
					Task.Run(() => interaction.TryCancel());
				}
			}
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue is IInteraction oldInteraction) oldInteraction.Completed -= OnInteractionCompleted;
			if (e.NewValue is IInteraction newInteraction) newInteraction.Completed += OnInteractionCompleted;
		}

		private void OnInteractionCompleted(object sender, InteractionCompletedEventArgs e)
		{
			if (sender is IInteraction interaction) interaction.Completed -= OnInteractionCompleted;

			Dispatcher.Invoke(() =>
			{
				try
				{
					DialogResult = e.Result;
				}
				catch
				{
					mPassDialogResultCheckDuringClose = true;
					Close();
				}
			});
		}
	}
}
