using System.IO;
using System.Threading.Tasks;
using PaymentAPI.Drivers;
using Newtonsoft.Json.Linq;
using PaymentAPI.Helpers;
using TechTalk.SpecFlow;
using PaymentAPI.Steps;

namespace PaymentAPI.Steps;


[Binding]
public sealed class APIRequestStepDef
{
    private readonly ScenarioContext _scenarioContext;
    private readonly CommonStepDef _commonSteps;
    private readonly RequestDriver _requestDriver;

    public APIRequestStepDef(
        ScenarioContext scenarioContext,
        RequestDriver requestDriver)
    {
        _scenarioContext = scenarioContext;
        _requestDriver = requestDriver;
        _commonSteps = new CommonStepDef(scenarioContext, _requestDriver);
    }


    [When(@"a POST request is performed")]
    public async Task WhenAPostRequestIsPerformed()
    {
        await _requestDriver.PerformPost();
    }

    [When(@"a POST request is performed to '(.*)' using payload '(.*)'")]
    public async Task WhenAPostRequestIsPerformedToUsingPayload(string url, string fileName)
    {
        _requestDriver.ResourceLocation = FormatPlaceholders(url);

        string path =
            Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName + "/Data",
                fileName);
        JObject payload = JObject.Parse(await File.ReadAllTextAsync(path));

        _scenarioContext["payload"] = payload;

        await _requestDriver.PerformPost();
    }

    /// <summary>
    /// This isn't ideal but can be used to substitute placeholders for values from scenarioContext
    /// </summary>
    private string FormatPlaceholders(string url)
    {
        return url
            .Replace("{recipientId}", _scenarioContext["BaseRecipientId"].ToString());
    }

    [Given(@"a ""(.*)"" transaction is performed")]
    public async Task GivenATransactionIsPerformed(string filename)
    {
        _commonSteps.GivenTheUserHasAValidJwtAndUniqueIdempotencyKey();
        _commonSteps.TheUserPreparesThePayload(filename);
        await WhenAPostRequestIsPerformed();
    }





    [Given(@"the message is encrypted in hMac256")]
    public void GivenTheMessageIsEncryptedWithHMAC256()
    {
        var hMacHash = new HMacHash(_scenarioContext["API_KEY"].ToString(), _scenarioContext["payload"].ToString());

        _scenarioContext["X-Message-Hash"] = hMacHash.ComputeHash();
        ;
    }
}