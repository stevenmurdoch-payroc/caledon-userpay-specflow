using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
namespace Common.Specs.Hooks
{    [Binding]
	public class Hooks
	{
		private readonly ScenarioContext _scenarioContext;
		public Hooks(ScenarioContext scenarioContext)
		{
			_scenarioContext = scenarioContext;
		}
		[BeforeScenario]
		public void WhenSetupEnvironment()
		{
			_scenarioContext["API_URL"] = Environment.GetEnvironmentVariable("API_URL");
			_scenarioContext["API_KEY"] = Environment.GetEnvironmentVariable("API_KEY");
			_scenarioContext["API_USER_ID"] = Environment.GetEnvironmentVariable("API_USER_ID");
		}
	}
	//  [BeforeTestRun]
	//  public static void BeforeTestRun()
	//  {
	//       Service.Instance.ValueRetrievers.Register(new NullValueRetriever("<NULL>"));
	//   }
	// }
}