using ComplexAppArchitectureMvvm.Services;
using ComplexAppArchitectureMvvm.Stores;
using System.ComponentModel;
using System.Windows;

namespace ComplexAppArchitectureMvvm.Windows
{
	public partial class Display : Window, INotifyPropertyChanged
	{
		private readonly IExitService mExitService;

		public Display() : this(null) { }
		public Display(IExitService exitService)
		{
			InitializeComponent();

			mExitService = exitService;

			DataContextChanged += OnDataContextChanged;
			Closing += OnClosing;
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			if (mExitService == null) return;

			mExitService.RequestExit();
			e.Cancel = true;
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public INavigationable Main => (DataContext as NavigationStore)?.Read();

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue is NavigationStore oldStore) oldStore.ValueChanged -= OnMainNavigationStoreValueChanged;
			if (e.NewValue is NavigationStore newStore) newStore.ValueChanged += OnMainNavigationStoreValueChanged;

			RaiseMainPropertyChanged();
		}
		private void OnMainNavigationStoreValueChanged(object sender, System.EventArgs e)
		{
			RaiseMainPropertyChanged();
		}
		private void RaiseMainPropertyChanged()
		{
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Main)));
		}
	}
}
