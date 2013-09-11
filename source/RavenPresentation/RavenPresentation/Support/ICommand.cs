using System;
using System.IO;
using NDesk.Options;
using System.Collections.Generic;
using System.Linq;

namespace Support
{
	public interface ICommand
	{
		void Execute(string[] commandLineArguments);
		void PrintHelp(TextWriter writer);
	}

	public abstract class Command : ICommand
	{
		OptionSet options; 

		protected void SetOptions(OptionSet options) 
		{
			this.options = options;
		}

		protected abstract void Execute();

		protected void EnsureArgument(string argument, string name) 
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				throw new ArgumentException("Please enter a value for the argument --" + name);
			}
		}

		protected string Prompt(string prompt, bool required = true, Func<string, bool> validate = null) 
		{
			while (true)
			{
				Console.Write(prompt + " ");
				var response = Console.ReadLine();

				if (required && string.IsNullOrWhiteSpace(response))
				{
					continue;
				}

				if (validate != null && !validate(response))
				{
					continue;
				}

				return response;
			}
		}

		protected string Choose(string prompt, params string[] choices) 
		{
			return Choose<string>(prompt, choices.ToDictionary(c => c, c => new Func<string>(() => c)));
		}

		protected T Choose<T>(string prompt, Dictionary<string, Func<T>> choices)
		{
			Console.WriteLine(prompt);

			var list = choices.ToList();
			var i = 1;
			foreach (var pair in choices)
			{
				Console.WriteLine(i + ". " + pair.Key);
				i++;
			}

			while (true)
			{
				Console.WriteLine("Please enter a number from {0} to {1}", 1, choices.Count);

				var choice = Console.ReadLine();
				int parsed;
				if (!int.TryParse(choice, out parsed))
				{
					continue;
				}

				parsed = parsed - 1;

				if (parsed >= choices.Count || parsed < 0)
				{
					continue;
				}

				var chosen = list[parsed];
				return chosen.Value();
			}
		}

		protected void If<T>(object instance, Action<T> callback) where T : class
		{
			var typed = instance as T;
			if (typed == null)
				return;

			callback(typed);
		}

		public void Execute(string[] commandLineArguments)
		{
			if (options != null)
			{
				var leftOver = options.Parse(commandLineArguments);
				if (leftOver.Count > 0)
				{
					throw new ArgumentException("Unrecognized arguments: " + string.Join(" ", leftOver));
				}
			}

			Execute();
		}

		public void PrintHelp(TextWriter writer)
		{
			if (options != null)
			{
				options.WriteOptionDescriptions(writer);
			}
		}
	}
	
	public class CommandDescriptor
	{
		string name;
		string description;
		Func<ICommand> commandFactory;

		public CommandDescriptor(string name, string description, Func<ICommand> commandFactory)
		{
			this.commandFactory = commandFactory;
			this.description = description;
			this.name = name;
		}

		public string Name {
			get
			{
				return this.name;
			}
		}

		public string Description {
			get
			{
				return this.description;
			}
		}

		public Func<ICommand> CommandFactory {
			get
			{
				return this.commandFactory;
			}
		}
	}
}

