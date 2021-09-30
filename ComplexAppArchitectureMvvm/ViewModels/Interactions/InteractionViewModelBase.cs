using ComplexAppArchitectureMvvm.Services;
using System;
using System.Mvvm;

namespace ComplexAppArchitectureMvvm.ViewModels.Interactions
{
	public abstract class InteractionViewModelBase : ViewModelBase, IInteraction
	{
		private bool mCompleted;

		public InteractionViewModelBase()
		{
			CancelCmd = Command.Factory.Create(() => TryCancel(), CanCancel);
		}

		public event EventHandler<InteractionCompletedEventArgs> Completed = delegate { };

		public Command CancelCmd { get; }

		public virtual bool TryCancel()
		{
			if (!CanCancel()) return false;
			MarkCompleted(false);
			return true;
		}

		protected virtual bool CanCancel() => true;
		protected void MarkCompleted(bool result)
		{
			lock (this)
			{
				if (mCompleted) return;

				mCompleted = true;
				Completed(this, new InteractionCompletedEventArgs(result));
			}
		}
	}
}
