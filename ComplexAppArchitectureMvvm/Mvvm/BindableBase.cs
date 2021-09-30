using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Mvvm
{
	public abstract class BindableBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected SetPropertyResult SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(storage, value)) return new SetPropertyResult(false);

			storage = value;
			RaisePropertyChangedEvent(propertyName);
			return new SetPropertyResult(true);
		}

		protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
	}

	public class SetPropertyResult
	{
		public SetPropertyResult(bool wasChanged)
		{
			WasChanged = wasChanged;
		}

		public bool WasChanged { get; }

		public void OnChanged(Action action)
		{
			if (WasChanged) action?.Invoke();
		}
		public void OnNotChanged(Action action)
		{
			if (!WasChanged) action?.Invoke();
		}
	}
}
