using System;
using System.Collections.Generic;
using System.Text;
using BSCore.Chat;

public class ChatCommandController
{
	private class CommandHandler
	{
		public Func<string[], bool> Handler;

		public string Description;

		public string[] Modes;

		public CommandHandler(Func<string[], bool> handler, string description, string[] modes)
		{
			Handler = handler;
			Description = description;
			Modes = modes;
		}
	}

	private static readonly string[] HELP_MODES = new string[1] { "<command> - Displays detailed information about this command" };

	public readonly ChatClient ChatClient;

	private readonly Dictionary<string, CommandHandler> _commandHandlers = new Dictionary<string, CommandHandler>();

	public ChatCommandController(ChatClient chatClient)
	{
		ChatClient = chatClient;
		RegisterChatCommand("help", HelpCommandHandler, "Lists available commands", HELP_MODES);
	}

	public void RegisterChatCommand(string command, Func<string[], bool> handler, string description, string[] modes)
	{
		_commandHandlers.Add(command, new CommandHandler(handler, description, modes));
	}

	public void HandleChatCommand(string message)
	{
		string[] array = message.Split(' ');
		string command = array[0].TrimStart('/').ToLower();
		string[] array2;
		if (array.Length > 1)
		{
			array2 = new string[array.Length - 1];
			for (int i = 1; i < array.Length; i++)
			{
				array2[i - 1] = array[i];
			}
		}
		else
		{
			array2 = new string[0];
		}
		HandleCommand(command, array2);
	}

	private void SendSystemMessage(string message)
	{
		ChatClient.SystemChatRoom.OnMessageReceived(new SystemMessage(message));
	}

	private void HandleCommand(string command, string[] args)
	{
		if (_commandHandlers.TryGetValue(command, out var value))
		{
			if (!value.Handler(args))
			{
				SendSystemMessage("Error running \"/" + command + "\" command. Type \"/help " + command + "\" for more info.");
			}
		}
		else
		{
			SendSystemMessage("Unknown command: \"" + command + "\". Type /help for a list of available commands.");
		}
	}

	private bool HelpCommandHandler(string[] args)
	{
		if (args.Length != 0)
		{
			if (_commandHandlers.TryGetValue(args[0], out var value))
			{
				StringBuilder stringBuilder = new StringBuilder("/" + args[0] + " - " + value.Description);
				if (value.Modes.Length != 0)
				{
					stringBuilder.Append("\n" + string.Join("\n", value.Modes));
				}
				SendSystemMessage(stringBuilder.ToString());
			}
			else
			{
				SendSystemMessage("Unknown command: " + args[0] + ". Type \"/help\" for a list of available commands.");
			}
		}
		else
		{
			SendSystemMessage("Available commands. Type \"/help command\" for more information on a specific command.");
			foreach (KeyValuePair<string, CommandHandler> commandHandler in _commandHandlers)
			{
				SendSystemMessage("/" + commandHandler.Key + " " + commandHandler.Value.Description);
			}
		}
		return true;
	}
}
