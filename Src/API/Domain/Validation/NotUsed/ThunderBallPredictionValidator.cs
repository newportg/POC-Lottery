//using FluentValidation;
//using Models;
//using System.Collections.Generic;
//using System.Linq;

//namespace Validation
//{
//    public class ThunderBallPredictionValidator : AbstractValidator<Prediction>
//    {
//        public ThunderBallPredictionValidator()
//        {
//            RuleFor(x => x.Selections).NotNull();
//            RuleFor(x => x.Selections).Must(SelectionsAreUnique);

//            // Apply Rules
//            // Get the highest ranked RG Pattern
//            // Get the OddEven Ball 
//            // Get BallTotalAverage
//            // Selection has not already been drawn
//            // Selection is not consecutive

//            // All Selections are unique;

//        }

//        private bool SelectionsAreUnique(List<Selection> arg)
//        {
//            return arg.Distinct().Count() == arg.Count();
//        }
//    }
//}
