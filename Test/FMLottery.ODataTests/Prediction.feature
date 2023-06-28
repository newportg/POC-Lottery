#Feature: Prediction
#	In order to to win at Thunderball
#	As a User
#	I want to be able to store and retrieve prediction information
#
#@prediction
##Scenario: Next Prediction
##	Given A connection to the Prediction Repository
##	When I ask for a list specifying the DrawNumber of 1
##	Then The list Should contain DrawNumber-1 elements
##	And The Prediction should be for DrawNumber 2
#
#Scenario: List Prediction information
#	Given A connection to the Prediction store
#	And I Create a list of Predictions with the DrawNumber of 9999 
#	And Save the list of predictions
#	When I ask for a list by specifying the DrawNumber of 9999
#	Then The result should be a list containing all of the predictions for DrawNumber 9999
#
##Scenario: Save a new Prediction
##	Given A connection to the Prediction store
##	And I Create a list of Predictions with the DrawNumber of 9999
##	When I Save the Predictions
##	Then The result should be HTTP-Status 204
##
##Scenario: Delete a Prediction
##	Given A connection to the Prediction store
##	And I Create a list of Predictions with the DrawNumber of 9999 and Save it
##	When I Delete the Predictions for DrawNumber 9999
##	Then The result should be HTTP-Status 204