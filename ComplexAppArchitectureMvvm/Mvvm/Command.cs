using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace System.Mvvm
{
	public class Command : ICommand, INotifyPropertyChanged
	{
		public static class Factory
		{
			public static Command Create(Action execute, Func<bool> canExecute = null) => new Command(execute, canExecute);
			public static Command<TParam> Create<TParam>(Action<TParam> execute, Func<TParam, bool> canExecute = null) => new Command<TParam>(execute, canExecute);
			public static CommandAsync Create(Func<Task> execute, Func<bool> canExecute = null) => new CommandAsync(execute, canExecute);
			public static CommandAsync<TParam> Create<TParam>(Func<TParam, Task> execute, Func<TParam, bool> canExecute = null) => new CommandAsync<TParam>(execute, canExecute);
		}

		public static Dispatcher RaiseChangedEventDispatcher { get; set; }

		private readonly Action mExecute;
		private readonly Func<bool> mCanExecute;

		private bool mIsEnabled;

		public Command(Action execute, Func<bool> canExecute = null)
		{
			mExecute = execute;
			mCanExecute = canExecute;

			IsEnabled = canExecute == null;
		}

		public event EventHandler CanExecuteChanged = delegate { };
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public bool IsEnabled
		{
			get => mIsEnabled;
			protected set
			{
				if (mIsEnabled == value) return;

				mIsEnabled = value;
				RaisePropertyChanged();
				RaiseCanExecuteChanged();
			}
		}

		public virtual bool CanExecute(object parameter)
		{
			IsEnabled = mCanExecute?.Invoke() ?? true;
			return IsEnabled;
		}

		public virtual void Execute(object parameter)
		{
			mExecute?.Invoke();
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		public void RaiseCanExecuteChanged()
		{
			if (RaiseChangedEventDispatcher != null && Thread.CurrentThread != RaiseChangedEventDispatcher.Thread)
			{
				RaiseChangedEventDispatcher.BeginInvoke((Action)(() => CanExecuteChanged(this, EventArgs.Empty)));
			}
			else
			{
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}
	}
	public class Command<TParameterType> : Command
	{
		private readonly Action<TParameterType> mExecute;
		private readonly Func<TParameterType, bool> mCanExecute;

		public Command(Action<TParameterType> execute, Func<TParameterType, bool> canExecute = null) : base(null, null)
		{
			mExecute = execute;
			mCanExecute = canExecute;

			IsEnabled = canExecute == null;
		}

		public override bool CanExecute(object parameter) => CanExecute((TParameterType)parameter);
		public virtual bool CanExecute(TParameterType parameter)
		{
			IsEnabled = mCanExecute?.Invoke(parameter) ?? true;
			return IsEnabled;
		}

		public override void Execute(object parameter) => Execute((TParameterType)parameter);
		public virtual void Execute(TParameterType parameter)
		{
			mExecute?.Invoke(parameter);
		}
	}

	public interface ICommandAsync : ICommand
	{
		bool IsExecuting { get; }
	}
	public class CommandAsync : Command, ICommandAsync
	{
		private readonly Func<Task> mExecute;

		private bool mIsExecuting;

		public CommandAsync(Func<Task> execute, Func<bool> canExecute = null) : base(null, canExecute)
		{
			mExecute = execute;
		}

		public bool IsExecuting
		{
			get => mIsExecuting;
			protected set
			{
				if (mIsExecuting == value) return;
				mIsExecuting = value;

				RaisePropertyChanged();
				RaiseCanExecuteChanged();
			}
		}

		public override bool CanExecute(object parameter)
		{
			IsEnabled = !IsExecuting && base.CanExecute(parameter);
			return IsEnabled;
		}
		public override async void Execute(object parameter)
		{
			IsExecuting = true;
			await mExecute?.Invoke();
			IsExecuting = false;
		}
	}
	public class CommandAsync<TParameterType> : Command<TParameterType>, ICommandAsync
	{
		private readonly Func<TParameterType, Task> mExecute;

		private bool mIsExecuting;

		public CommandAsync(Func<TParameterType, Task> execute, Func<TParameterType, bool> canExecute = null) : base(null, canExecute)
		{
			mExecute = execute;
		}

		public bool IsExecuting
		{
			get => mIsExecuting;
			protected set
			{
				if (mIsExecuting == value) return;
				mIsExecuting = value;

				RaisePropertyChanged();
				RaiseCanExecuteChanged();
			}
		}

		public override bool CanExecute(TParameterType parameter)
		{
			IsEnabled = !IsExecuting && base.CanExecute(parameter);
			return IsEnabled;
		}
		public override async void Execute(TParameterType parameter)
		{
			IsExecuting = true;
			await mExecute?.Invoke(parameter);
			IsExecuting = false;
		}
	}
}
