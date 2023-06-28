Feature: ThunderBallSelectionValidator
	In win the lottery
	As a user
	I validate my selections

@mytag
Scenario: Validate my selections
	Given I know the ThunderBall rules
	And I have a valid ThunderBall object
	And I have a valid RenatoGianella object
	When I validate my selections 
	Then the result should be positive
