Feature: Userpay Transactions 
    
    Scenario: Perform a Fee Calculation Transaction
		
        Given the user prepares the Payment API "FeeCalculate" request       
        When a POST request is performed
        Then the response code will be response code '200'

	Scenario: Perform a Fee Payment Transaction
		
		Given the user prepares the Payment API "FeePayment" request       
		When a POST request is performed
		Then the response code will be response code '200'
		
	Scenario: Perform a Fee Void Transaction
		
		Given the user prepares the Payment API "FeeVoid" request       
		When a POST request is performed
		Then the response code will be response code '200'