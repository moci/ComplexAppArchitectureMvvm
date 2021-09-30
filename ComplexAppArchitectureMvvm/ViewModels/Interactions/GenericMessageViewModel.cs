using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvvm;
using static ComplexAppArchitectureMvvm.ViewModels.Interactions.InputCommand;

namespace ComplexAppArchitectureMvvm.ViewModels.Interactions
{
	public class GenericMessageViewModel : InteractionViewModelBase
	{
		public MessageType Message { get; }

		public static class Factory
		{
			public static GenericMessageViewModel CreateInformational(MessageType message)
			{
				return Create(message, InputType.OK);
			}
			public static GenericMessageViewModel CreateConfirm(MessageType message)
			{
				return Create(message, InputType.YES, InputType.NO);
			}
			public static GenericMessageViewModel CreateConfirmCancel(MessageType message)
			{
				return Create(message, InputType.YES, InputType.NO, InputType.CANCEL);
			}
			public static GenericMessageViewModel Create(MessageType message, params InputType[] allowedInteractions)
			{
				return new GenericMessageViewModel(message, allowedInteractions);
			}
		}

		protected GenericMessageViewModel(MessageType message, IEnumerable<InputType> allowedInteractions)
		{
			Message = message;

			Inputs = (allowedInteractions ?? new[] { InputType.OK }).Distinct().Select(type =>
			{
				return type switch
				{
					InputType.OK => new InputCommand(InputType.OK, true, HandleInput),
					InputType.CANCEL => new InputCommand(InputType.CANCEL, false, HandleInput),
					InputType.YES => new InputCommand(InputType.YES, true, HandleInput),
					InputType.NO => new InputCommand(InputType.NO, false, HandleInput),
					_ => throw new NotImplementedException(),
				};
			}).ToArray();

			// So we've got the regular cancel command, lets remap it to the first 'negative' result (or the single available result)
			CancelCmd = Inputs.FirstOrDefault(input => input.InteractionResult == false) ?? Inputs.First();
		}

		public IEnumerable<InputCommand> Inputs { get; }
		public InputType UsedInput { get; private set; }
		public new InputCommand CancelCmd { get; private set; }

		private void HandleInput(InputType type, bool result)
		{
			UsedInput = type;
			MarkCompleted(result);
		}
	}

	public class InputCommand : Command
	{
		public InputCommand(InputType type, bool result, Action<InputType, bool> execute) : base(() => execute(type, result))
		{
			Type = type;
			InteractionResult = result;
		}

		public InputType Type { get; }
		public bool InteractionResult { get; }

		public enum InputType
		{
			OK,
			CANCEL,
			YES,
			NO
		}
	}

	public enum MessageType
	{
		COMPLETED,
		NOT_AUTHORIZED,
	}
}
