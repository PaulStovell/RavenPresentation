using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{
	public class ListResponsesCommand : Command
	{
		const int itemsPerPage = 5;
		IDocumentStore store;

		public ListResponsesCommand(IDocumentStore store)
		{
			this.store = store;
		}

		protected override void Execute()
		{
			var survey = GetSurvey();

			var skip = 0;
			while (true)
			{
				PrintPageOfResponses(survey, skip);

				Console.WriteLine("Press enter to go the next page, or 'q' to quit");
				var line = Console.ReadLine();
				if (line == "q")
					break;

				skip += itemsPerPage;
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

		void PrintPageOfResponses(Survey survey, int skip)
		{
			using (var session = store.OpenSession())
			{
				var responses = session.Query<Response>()
					.Where(r => r.SurveyId == survey.Id)
					.Skip(skip)
					.Take(itemsPerPage)
					.ToList();

				foreach (var response in responses)
				{
					Console.WriteLine("Response: " + response.Id);
					foreach (var question in survey.Questions)
					{
						Console.WriteLine("{0,-30}  {1,-47}", question.QuestionText, response.Answers[question.Id]);
					}

					Console.WriteLine();
				}
			}
		}
	}
	
}
