using System;
namespace CodePulse.Models.Domain
{
	public class Search
	{
		public Guid Id { get; set; }

		public string keyword { get; set; }

		public string result { get; set; }

		public string type { get; set; }
	}
}

