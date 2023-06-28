using FluentAssertions;
using FluentValidation.Results;
using Models;
using Rules;
using Validation;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace CoreTests
{
    [Binding]
    public class ThunderBallSelectionValidatorSteps
    {
        private ThunderBallRules _rules;
        private Models.ThunderBall _thunderball;
        private RenatoGianella _renatoGianella;

        private ThunderBallSelectionValidator _validator;
        private Selection _selection;
        private ValidationResult _results;

        [Given(@"I know the ThunderBall rules")]
        public void GivenIKnowTheThunderBallRules()
        {
            _rules = new ThunderBallRules();
        }

        [Given(@"I have a valid ThunderBall object")]
        public void GivenIHaveAValidThunderBallObject()
        {
            _thunderball = new Models.ThunderBall
            {
                Draws = new List<ThunderBallDto>
                {
                    new ThunderBallDto
                    {
                        DrawDate = DateTime.Today,
                        BallTotal = 15,
                        NumOddBalls = 3,
                        RenatoGianellaPattern = new int[4] { 5, 0, 0, 0 },
                        BallSet = "1",
                        Machine = "Test",
                        DrawNumber = "9999",
                        Balls = new List<int>() { 1, 2, 3, 4, 5 },
                        BonusBalls = new List<int> { 1 }
                    }
                },
                BallTotalAvg = 15,
                NumBallDrawings = new List<Ball>
                {
                    new Ball
                    {
                        BallNumber = "1",
                        Count = 1,
                        Draws= new List<string>() { "9999" }
                    },
                    new Ball
                    {
                        BallNumber = "2",
                        Count = 1,
                        Draws= new List<string>() { "9999" }
                    },
                    new Ball
                    {
                        BallNumber = "3",
                        Count = 1,
                        Draws= new List<string>() { "9999" }
                    },
                    new Ball
                    {
                        BallNumber = "4",
                        Count = 1,
                        Draws= new List<string>() { "9999" }
                    },
                    new Ball
                    {
                        BallNumber = "5",
                        Count = 1,
                        Draws= new List<string>() { "9999" }
                    }
                },
                OddBalls = new int[6] { 0, 0, 0, 3, 0, 0 },
                RenatoGianellaOccurrance = new List<RenatoGianella>()
                {
                    new RenatoGianella()
                    {
                        Name = "5000",
                        Pattern = new int[4] { 5, 0, 0, 0 },
                        Count = 1,
                        Draws = new List<string>() { "9999" }
                    }
                }
            };

            var tbValidator = new ThunderBallValidator();
            ValidationResult results = tbValidator.Validate(_thunderball);
            results.IsValid.Should().BeTrue();
        }

        [Given(@"I have a valid RenatoGianella object")]
        public void GivenIHaveAValidRenatoGianellaObject()
        {
            _renatoGianella = new RenatoGianella()
            {
                Name = "5000",
                Pattern = new int[4] { 5, 0, 0, 0 },
                Count = 1,
                Draws = new List<string>() { "9999" }
            };

            var rgValidator = new RenatoGianellaValidator();
            ValidationResult results = rgValidator.Validate(_renatoGianella);
            results.IsValid.Should().BeTrue();
        }

        [When(@"I validate my selections")]
        public void WhenIValidateMySelections()
        {
            _selection = new Selection(_rules, _renatoGianella);
            _validator = new ThunderBallSelectionValidator(_rules, _thunderball);
            _results = _validator.Validate(_selection);
        }
        
        [Then(@"the result should be positive")]
        public void ThenTheResultShouldBePositive()
        {
            _results.IsValid.Should().BeTrue();
        }
    }
}
