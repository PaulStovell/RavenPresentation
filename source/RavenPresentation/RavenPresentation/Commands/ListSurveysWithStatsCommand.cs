using System;
using Raven.Client;
using Support;
using NDesk.Options;
using Model;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Indexes;

namespace Commands
{
	public class ListSurveysWithStatsCommand : Command 
	{
		IDocumentStore store;

		public ListSurveysWithStatsCommand(IDocumentStore store)
		{
			this.store = store;
		}
		
		protected override void Execute()
		{
			using (var session = store.OpenSession())
			{
				var surveys = session.Query<SurveyWithResponseCountIndex.Result, SurveyWithResponseCountIndex>()
					.TransformWith<SurveyTitleTransformer, SurveyWithResponseCountIndex.Result>()
					.ToList();

				PrintSurveys(surveys);
			}
		}

		void PrintSurveys(IEnumerable<SurveyWithResponseCountIndex.Result> surveys) 
		{
			Console.WriteLine("{0,-10}  {1,-50}  {2,5}", "ID", "Title", "Responses");
			Console.WriteLine(new String('-', 77));
			foreach (var survey in surveys)
			{
				Console.WriteLine("{0,-10}  {1,-50}  {2,5}", survey.Id, survey.Title, survey.NumberOfResponses);
			}
		}
	}
	
}
