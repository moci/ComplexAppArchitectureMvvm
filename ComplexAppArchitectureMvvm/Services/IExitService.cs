using ComplexAppArchitectureMvvm.Domain;
using System;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface IExitService
	{
		void RequestExit(ExitType type);
		void RequestExit();
	}

	public class ExitService : IExitService
	{
		public event EventHandler<ExitRequestedEventArgs> ExitRequested = delegate { };

		public void RequestExit(ExitType type)
		{
			ExitRequested(this, new ExitRequestedEventArgs(type));
		}
		public void RequestExit()
		{
			ExitRequested(this, new ExitRequestedEventArgs(null));
		}
	}

	public class ExitRequestedEventArgs : EventArgs
	{
		public ExitRequestedEventArgs(ExitType? type)
		{
			Type = type;
		}

		public ExitType? Type { get; }
	}
}
