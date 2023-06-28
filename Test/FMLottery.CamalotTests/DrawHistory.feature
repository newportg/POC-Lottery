Feature: DrawHistory
	In view or analyse lottery data
	As a application
	I want to be able to receive lottery draw history data

@mytag
Scenario: Get ThunderBall Draw History
	Given I ama lottery application
	When I make a request for lottery draw history 
	Then Then I will have a List of lottery draws.