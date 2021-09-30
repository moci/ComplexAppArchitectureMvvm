using System;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.Services
{
	public interface IInteraction
	{
		event EventHandler<InteractionCompletedEventArgs> Completed;

		bool TryCancel();
	}

	public class InteractionCompletedEventArgs : EventArgs
	{
		public InteractionCompletedEventArgs(bool result)
		{
			Result = result;
		}

		public bool Result { get; }
	}

	public interface IInteractionService
	{
		Task<bool> Prompt(IInteraction interaction);
		void Fork(IInteraction interaction);
	}

	public class InteractionService : IInteractionService
	{
		private readonly Action<IInteraction> mPromptHandler;
		private readonly Action<IInteraction> mForkHandler;

		public InteractionService(Action<IInteraction> promptHandler, Action<IInteraction> forkHandler)
		{
			mPromptHandler = promptHandler ?? throw new ArgumentNullException(nameof(promptHandler));
			mForkHandler = forkHandler ?? throw new ArgumentNullException(nameof(forkHandler));
		}

		public Task<bool> Prompt(IInteraction interaction)
		{
			interaction = interaction ?? throw new ArgumentNullException();

			var tcs = new TaskCompletionSource<bool>();

			void completedHandler(object sender, InteractionCompletedEventArgs e)
			{
				((IInteraction)sender).Completed -= completedHandler;
				tcs.SetResult(e.Result);
			}

			Task.Run(() =>
			{
				if (interaction == null)
				{
					tcs.SetResult(true);
					return;
				}

				interaction.Completed += completedHandler;
				mPromptHandler(interaction);
			});

			return tcs.Task;
		}

		public void Fork(IInteraction interaction)
		{
			interaction = interaction ?? throw new ArgumentNullException();

			void completedHandler(object sender, InteractionCompletedEventArgs e)
			{
				((IInteraction)sender).Completed -= completedHandler;
				((IDisposable)sender)?.Dispose();
			}

			interaction.Completed += completedHandler;
			mForkHandler(interaction);
		}
	}
}
