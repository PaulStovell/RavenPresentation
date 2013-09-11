using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{

	public class TakeSurveyCommand : Command 
	{
		IDocumentStore store;

		public TakeSurveyCommand(IDocumentStore store)
		{
			this.store = store;
		}

		protected override void Execute()
		{
			var survey = GetSurvey();
			Console.WriteLine("Taking survey: " + survey.Title);

			var answers = GatherAnswers(survey);

			SaveResponse(survey, answers);

			Console.WriteLine("Thanks for taking the survey!");
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

		Dictionary<string, string> GatherAnswers(Survey survey) 
		{
			var result = new Dictionary<string, string>();

			var questionNumber = 1;
			foreach (var question in survey.Questions)
			{
				Console.WriteLine("Question " + questionNumber + ": ");
				Console.WriteLine(question.QuestionText);

				string response = null;

				If<TextBoxSurveyQuestion>(question, q => response = TakeTextBoxSurveyQuestion(q));
				If<MultipleChoiceSurveyQuestion>(question, q => response = TakeMultipleChoiceSurveyQuestion(q));

				Console.WriteLine();

				result[question.Id] = response;

				questionNumber++;
			}

			return result;
		}

		string TakeTextBoxSurveyQuestion(TextBoxSurveyQuestion question) 
		{
			return Prompt("Answer:");
		}

		string TakeMultipleChoiceSurveyQuestion(MultipleChoiceSurveyQuestion question) 
		{
			return Choose("Choices:", question.Choices);
		}

		void SaveResponse(Survey survey, Dictionary<string, string> answers) 
		{
			var response = new Response();
			response.SurveyId = survey.Id;
			response.ResponseGiven = DateTimeOffset.UtcNow;
			response.Answers = answers;
		
			using (var session = store.OpenSession())
			{
				session.Store(response);
				session.SaveChanges();
			}
		}
	}
	
}
