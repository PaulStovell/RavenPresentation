using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{
	public class AutoRespondCommand : Command 
	{
		IDocumentStore store;
		Random rand = new Random();

		public AutoRespondCommand(IDocumentStore store)
		{
			this.store = store;
		}

		protected override void Execute()
		{
			var survey = GetSurvey();

			using (var insert = store.BulkInsert())
			{
				for (var i = 0; i < 2000; i++)
				{
					var response = GenerateRandomResponse(survey);
					insert.Store(response);

					if (i % 100 == 0)
					{
						Console.WriteLine("Responses generated so far: " + i);
					}
				}
			}
		}

		Survey GetSurvey() 
		{
			using (var session = store.OpenSession())
			{
				var id = Prompt("Survey ID:");
				var survey = session.Load<Survey>(id);
				if (survey == null)
				{
					Console.WriteLine("Survey not found: " + id);
					return GetSurvey();
				}

				return survey;
			}
		}

		Response GenerateRandomResponse(Survey survey)
		{
			var answers = new Dictionary<string, string>();

			foreach (var question in survey.Questions)
			{
				If<TextBoxSurveyQuestion>(question, tb => answers[question.Id] = RandomString());
				If<MultipleChoiceSurveyQuestion>(question, tb => answers[question.Id] = RandomChoice(tb.Choices));
			}

			var response = new Response();
			response.SurveyId = survey.Id;
			response.ResponseGiven = DateTimeOffset.UtcNow;
			response.Answers = answers;
			return response;
		}

		string RandomString()
		{
			return RandomStringGenerator.Generate(10);
		}

		string RandomChoice(string[] choices)
		{
			return choices[rand.Next(0, choices.Length)];
		}
	}
	
}
