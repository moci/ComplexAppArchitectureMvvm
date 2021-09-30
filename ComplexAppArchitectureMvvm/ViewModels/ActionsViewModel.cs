using ComplexAppArchitectureMvvm.Services;
using ComplexAppArchitectureMvvm.ViewModels.Interactions;
using System;
using System.Mvvm;
using System.Threading.Tasks;

namespace ComplexAppArchitectureMvvm.ViewModels
{
	public class ActionsViewModel : ViewModelBase, INavigationable
	{
		public ActionsViewModel(Func<INavigationService> hubNavigationServiceProvider, IInteractionService interactionService, IAuthenticationService authenticationService)
		{
			async Task performProtectedAction(AccessType type)
			{
				if (!await authenticationService.TryGetAccess(type))
				{
					using var msg = GenericMessageViewModel.Factory.CreateInformational(MessageType.NOT_AUTHORIZED);
					await interactionService.Prompt(msg);
				}
				else
				{
					using var msg = GenericMessageViewModel.Factory.CreateInformational(MessageType.COMPLETED);
					interactionService.Fork(msg);
				}
			}

			ToHubCmd = Command.Factory.Create(() => hubNavigationServiceProvider().Navigate());
			ActionACmd = Command.Factory.Create(async () => await performProtectedAction(AccessType.ACTION_A));
			ActionBCmd = Command.Factory.Create(() => performProtectedAction(AccessType.ACTION_B));
		}

		public Command ToHubCmd { get; }
		public Command ActionACmd { get; }
		public Command ActionBCmd { get; }

		public void HandlePendingNavigation(PendingNavigationInfo info) { }
	}
}
