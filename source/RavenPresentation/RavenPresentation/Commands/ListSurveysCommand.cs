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
	public class ListSurveysCommand : Command 
	{
		IDocumentStore store;

		public ListSurveysCommand(IDocumentStore store)
		{
			this.store = store;
		}
		
		protected override void Execute()
		{
			using (var session = store.OpenSession())
			{
				var surveys = session.Query<Survey>();

				PrintSurveys(surveys);
			}
		}

		void PrintSurveys(IEnumerable<Survey> surveys) 
		{
			Console.WriteLine("{0,-10}  {1,-50}", "ID", "Title");
			Console.WriteLine(new String('-', 77));
			foreach (var survey in surveys)
			{
				Console.WriteLine("{0,-10}  {1,-50}", survey.Id, survey.Title);
			}
		}
	}
	
}
