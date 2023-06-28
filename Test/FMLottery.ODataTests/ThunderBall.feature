Feature: ThunderBall
	In order to to win at Thunderball
	As a User
	I want to be able to store and retrieve draw information

@thunderball
Scenario: List draw information
	Given A connection to the ThunderBall store
	And I Create a list of 2 draws and Save it
	When I ask for a list without specifing a DrawNumber
	Then The result should be a list containing 2 draws

Scenario: List UpTo draw information
	Given A connection to the ThunderBall store
	And I Create a LotteryDto object 9999 and Save it
	When I ask for a list specifing a DrawNumber 9999
	Then The result should be a list containing all the draws upto the DrawNumber 9999

Scenario: Get a draw
	Given A connection to the ThunderBall store
	And I Create a LotteryDto object 9999 and Save it
	When I ask for a individual draw 9999
	Then The result should be a list containing that draw 9999

Scenario: Bad DrawNumber
	Given A connection to the ThunderBall store
	When I ask for a individual draw ABCD
	Then The result should be null

Scenario: Save a new draw
	Given A connection to the ThunderBall store
	And I Create a LotteryDto object 9999
	When I Save the Draw
	Then The result should be HTTPStatus 204

Scenario: Delete a draw
	Given A connection to the ThunderBall store
	And I Create a LotteryDto object 9999 and Save it
	When I Delete the Draw 9999
	Then The result should be HTTPStatus 204