using PaymentAPI.Drivers;

namespace PaymentAPI.Steps;

[Binding]
public sealed class PaymentAPIStepDef
{
    private readonly TransactionStepDef _transactionStepDef;
    private readonly CommonStepDef _commonStepDef;
    private readonly APIRequestStepDef _aPIRequestStepDef;
    private readonly ApiResponseStepDef _aPIResponseStepDef;  
    
    public PaymentAPIStepDef(ScenarioContext scenarioContext, RequestDriver requestDriver, ResponseValidationDriver responseValidationDriver)
    {
        _transactionStepDef = new TransactionStepDef(scenarioContext);
        _commonStepDef = new CommonStepDef(scenarioContext, requestDriver);
        _aPIRequestStepDef = new APIRequestStepDef(scenarioContext, requestDriver);
        _aPIResponseStepDef = new ApiResponseStepDef(scenarioContext, responseValidationDriver);
    }

    [Given(@"the user prepares the Payment API ""(.*)"" request")]
    public void GivenTheUserPreparesThePaymentApiRequest(string requestType)
    {
        _transactionStepDef.GivenTheUserAttemptsToConnectToThePaymentApi();
        _commonStepDef.TheUserPreparesThePayload($"{requestType}.json");
        _aPIRequestStepDef.GivenTheMessageIsEncryptedWithHMAC256();
    }
    
    [Given(@"a ""(.*)"" transaction has been performed")]
    public async Task GivenATransactionHasBeenPerformed(string preReqTransaction)
    {
        GivenTheUserPreparesThePadApiRequest(preReqTransaction);
        await _aPIRequestStepDef.WhenAPostRequestIsPerformed();
    }


    [Given(@"the user prepares the API ""(.*)"" request")]
    public void GivenTheUserPreparesThePadApiRequest(string requestType)
    {
        _transactionStepDef.GivenTheUserAttemptsToConnectToThePaymentApi();
        _commonStepDef.TheUserPreparesThePadPayload($"{requestType}.json");
        _aPIRequestStepDef.GivenTheMessageIsEncryptedWithHMAC256();
    }
}
