using System;

namespace RavenPresentation
{
	public class Tweet
	{
		public string Id { get; set; }
		public string TweeterId { get; set; }

		public string Message { get; set; }

		public string[] MentionedTweeterIds { get; set; }
	}
}

