using System;
using Raven.Client.Document;
using System.Net;
using System.Linq;
using Support;
using System.Collections.Generic;
using Commands;
using Raven.Client.Indexes;
using System.Reflection;

namespace RavenTestDrive
{
	class MainClass
	{
		static readonly List<CommandDescriptor> commands = new List<CommandDescriptor>();

		public static int Main(string[] args)
		{
			Console.Clear();

			using (var store = new DocumentStore())
			{
				store.ParseConnectionString("Url = http://stovellmac:8080/;");
				store.DefaultDatabase = "Adnug";
				store.EnlistInDistributedTransactions = false;
				store.Initialize();

				// Prevent a bug in Mono/Raven
				store.JsonRequestFactory.DisableRequestCompression = true;

				IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), store);

				commands.Add(new CommandDescriptor("create-survey", "creates a new survey", () => new CreateSurveyCommand(store)));
				commands.Add(new CommandDescriptor("list-surveys", "lists surveys", () => new ListSurveysCommand(store)));
				commands.Add(new CommandDescriptor("survey-stats", "list response statistics for surveys", () => new ListSurveysWithStatsCommand(store)));
				commands.Add(new CommandDescriptor("take-survey", "take a survey", () => new TakeSurveyCommand(store)));
				commands.Add(new CommandDescriptor("autorespond", "generate responses to a survey", () => new AutoRespondCommand(store)));
				commands.Add(new CommandDescriptor("list-responses", "list responses to a survey", () => new ListResponsesCommand(store)));

				try
				{
					var firstArg = args.FirstOrDefault();
					var command = GetCommand(firstArg);

					command.Execute(args.Skip(1).ToArray());
					
					return 0;
				}
				catch (ArgumentException ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ResetColor();
					return -1;
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(ex);
					Console.ResetColor();
					return -2;
				}
			}
		}

		static ICommand GetCommand(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return new HelpCommand(commands);
			}

			var command = commands.FirstOrDefault(c => c.Name == name);
			if (command == null) 
			{
				return new HelpCommand(commands);
			}

			return command.CommandFactory();
		}

		static void PrintHelp()
		{
			Console.WriteLine("To run: surveyor.exe <command> [arguments]");
			Console.WriteLine();
			Console.WriteLine("Where <command> is one of:");
			Console.WriteLine(" create-survey     creates a new survey");
			Console.WriteLine(" list-survey       lists surveys");
			Console.WriteLine(" delete-survey     deletes a survey");
			Console.WriteLine(" print-survey      displays a survey");
			Console.WriteLine(" respond           lets you take a survey interactively");
			Console.WriteLine(" auto-respond      generates random responses to a survey");
			Console.WriteLine(" view-responses    lists responses to a survey");
			Console.WriteLine(" help              shows this help text");
		}
	}
}
