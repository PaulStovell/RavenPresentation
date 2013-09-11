using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{

	public class CreateSurveyCommand : Command
	{
		readonly IDocumentStore store;

		public CreateSurveyCommand(IDocumentStore store)
		{
			this.store = store;
		}

		protected override void Execute()
		{
			var id = Prompt("Enter a unique ID for this survey:", validate: EnsureIdNotInUse);

			var survey = new Survey();
			survey.Id = id;
			survey.Title = Prompt("Enter a title for this survey:");
		
			DefineSurveyQuestions(survey);

			using (var session = store.OpenSession())
			{
				session.Store(survey, id);

				session.SaveChanges();
			}
		}


		void DefineSurveyQuestions(Survey survey) 
		{
			int questionNumber = 1;
			while (true)
			{
				var question = CreateQuestion(questionNumber);
				if (question == null)
				{
					break;
				}
				else
				{
					survey.Questions.Add(question);
					questionNumber++;
				}
			}
		}
		
	    SurveyQuestion CreateQuestion(int questionNumber) 
		{
			Console.WriteLine("Define question " + questionNumber);
			var title = Prompt("Title:", required: false);
			if (string.IsNullOrWhiteSpace(title))
			{
				return null;
			}

			return Choose<SurveyQuestion>("Question type:", new Dictionary<string, Func<SurveyQuestion>> {
				{ 
					"Text", () => {
						return new TextBoxSurveyQuestion { QuestionText = title };
					}
				},
				{
					"Radio", () => {
						var choices = new List<string>();
						while (true) 
						{
							var choice = Prompt("Choice " + (choices.Count + 1) + ":", required: choices.Count < 2);
							if (string.IsNullOrWhiteSpace(choice)) 
								break;
							choices.Add(choice);
						}

						return new MultipleChoiceSurveyQuestion { QuestionText = title, Choices = choices.ToArray() };
					}
				},
				{
					"Cancel", () => {
						return null;
					}
				}
			});
		}

		bool EnsureIdNotInUse(string id)
		{	
			using (var session = store.OpenSession())
			{
				var existing = session.Load<Survey>(id);
				if (existing != null)
				{
					Console.WriteLine("A survey with ID " + id + " already exists");
					return false;
				}
			}

			return true;
		}
	}
}

