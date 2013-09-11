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
	public class SurveyWithResponseCountIndex : AbstractIndexCreationTask<Response, SurveyWithResponseCountIndex.Result>
	{
		public SurveyWithResponseCountIndex()
		{
			Map = responses => 
				from response in responses
				select new Result { Id = response.SurveyId, NumberOfResponses = 1 };

			Reduce = results => 
				from result in results
				group result by result.Id into g
				select new { Id = g.Key, NumberOfResponses = g.Sum(r => r.NumberOfResponses) };
		}

		public class Result 
		{
			public string Id { get; set; }
			public string Title { get; set; }
			public int NumberOfResponses { get; set; }
		}
	}

	public class SurveyTitleResponder : AbstractTransformerCreationTask<SurveyWithResponseCountIndex.Result> 
	{
		public SurveyTitleResponder()
		{
			TransformResults = results => 
				from result in results
				select new { Id = result.Id, Title = LoadDocument<Survey>(result.Id).Title, NumberOfResponses = result.NumberOfResponses };
		}
	}
}
