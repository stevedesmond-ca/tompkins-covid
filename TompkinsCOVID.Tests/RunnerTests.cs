using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace TompkinsCOVID.Tests;

public class RunnerTests
{
	[Fact]
	public async Task TweetsNewRecords()
	{
		//arrange
		var twitter = Substitute.For<ITwitter>();
		twitter.GetLatestPostedDate().Returns(DateTime.Parse("6/30/2021"));

		var hd = Substitute.For<IHealthDepartment>();
		var yesterday = new Record(Stub.Row(new[] { "06/30/2021" }));
		var today = new Record(Stub.Row(new[] { "07/01/2021" }));
		hd.GetLatestRecords(Arg.Any<string>()).Returns(new[] { yesterday, today });

		var settings = new Dictionary<string, string>
			{
				{ "url", "http://localhost" },
				{ "wait", "0" }
			};
		var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

		void Log(string s)
		{
		}

		var runner = new Runner(twitter, hd, Log, config);

		//act
		await runner.Run();

		//assert
		await twitter.Received(1).Tweet(Arg.Any<Record>());
	}
}