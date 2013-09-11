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
	public class SurveyTitleTransformer : AbstractTransformerCreationTask<SurveyWithResponseCountIndex.Result> 
	{
		public SurveyTitleTransformer()
		{
			TransformResults = results => 
				from result in results
				select new { Id = result.Id, Title = LoadDocument<Survey>(result.Id).Title, NumberOfResponses = result.NumberOfResponses };
		}
	}
}
