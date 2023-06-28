//using AutoMapper;
//using Models;
//using Rules;
//using Validation;
//using System.Collections.Generic;

//namespace Mapping
//{
//    public class ToThunderBallPredictionMap : Profile
//    {
//        private static ThunderBallRules _rules;

//        public ToThunderBallPredictionMap()
//        {
//            _rules = new ThunderBallRules();

//            // Add as many of these lines as you need to map your objects
//            CreateMap<Models.ThunderBall, Prediction>()
//                .ForMember(dst => dst.Selections, opt => opt.MapFrom(src => Prediction(src, _rules)));
//        }

//        private static List<Selection> Prediction(Models.ThunderBall src, ThunderBallRules rules)
//        {
//            List<Selection> selections = new List<Selection>();
//            var validator = new ThunderBallSelectionValidator(rules, src);

//            for (int i = 0; i < rules.NoOfGuesses(); i++)
//            {
//                // Select 5 balls + 1
//                Selection selection = new Selection(rules, src.RenatoGianellaOccurrance[src.RenatoGianellaOccurrance.Count - 1]);

//                // Validate
//                validator.Validate(selection);


//                // Apply Rules
//                // Get the highest ranked RG Pattern
//                // Get the OddEven Ball 
//                // Selection is not consecutive

//                // All Selections are unique;
//                // Get BallTotalAverage
//                // Selection has not already been drawn

//                // Add to Selections
//                selections.Add(selection);

//                // Save to DB
//            }

//            System.Console.WriteLine(rules.NoOfBalls);

//            return selections;
//        }
//    }
//}
