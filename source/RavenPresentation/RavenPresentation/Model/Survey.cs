using System;
using System.Collections.Generic;

namespace Model
{
	public class Survey
	{
		public Survey()
		{
			Questions = new List<SurveyQuestion>();
		}

		public string Id { get; set; }
		public string Title { get; set; }

		public List<SurveyQuestion> Questions { get; set; }
	}

	public abstract class SurveyQuestion 
	{
		public SurveyQuestion ()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }
		public string QuestionText { get; set; }
	}

	public class TextBoxSurveyQuestion : SurveyQuestion 
	{

	}

	public class MultipleChoiceSurveyQuestion : SurveyQuestion
	{
		public string[] Choices { get; set; }
	}

	public class Response 
	{
		public string Id { get; set; }
		public string SurveyId { get; set; }
		public DateTimeOffset ResponseGiven { get;set; }
		public Dictionary<string, string> Answers { get; set; }
	}
}

