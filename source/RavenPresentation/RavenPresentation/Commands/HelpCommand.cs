using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{
	public class HelpCommand : ICommand
	{
		List<CommandDescriptor> commands;

		public HelpCommand(List<CommandDescriptor> commands)
		{
			this.commands = commands;

		}

		public void Execute(string[] commandLineArguments)
		{
			var commandName = commandLineArguments.FirstOrDefault();
			if (string.IsNullOrWhiteSpace(commandName))
			{
				PrintHelp(Console.Out);
			}
			else
			{
				PrintHelpForCommand(commandName);
			}
		}

		public void PrintHelp(System.IO.TextWriter writer)
		{
			Console.WriteLine("Usage:  surveyor.exe <command> [args]");
			Console.WriteLine();

			Console.WriteLine("Where <command> is one of:");
			Console.WriteLine();
			foreach (var command in commands)
			{
				Console.WriteLine(command.Name + "\t\t" + command.Description);
			}
		}

		public void PrintHelpForCommand(string commandName)
		{
			var command = commands.FirstOrDefault(c => c.Name == commandName);
			if (command == null)
			{
				throw new ArgumentException("Unknown command: " + commandName);
			}

			Console.WriteLine("Usage:  surveyor.exe " + commandName + " [args]");
			Console.WriteLine();
			command.CommandFactory().PrintHelp(Console.Out);
		}
	}
	
}
