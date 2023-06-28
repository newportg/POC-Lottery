Feature: ThunderBallValidator
	In validate a thunderball draw
	As a tester
	I want to be told if a draw is invalid

@mytag
Scenario: Empty draw
	Given I have created a Empty Thunderball object
	When I validate it
	Then the result should be false

Scenario: Single draw
	Given I have created a Thunderball object
	When I validate it
	Then the result should be true
