//using FluentValidation;
//using Models;

//namespace Validation
//{
//    public class ThunderBallValidator : AbstractValidator<ThunderBall>
//    {
//        public ThunderBallValidator()
//        {
//            RuleFor(x => x.BallTotalAvg).GreaterThan(0);
//            RuleFor(x => x.Draws).NotNull();
//            RuleFor(x => x.NumBallDrawings).NotNull();
//            RuleFor(x => x.OddBalls).NotNull();
//            RuleFor(x => x.RenatoGianellaOccurrance).NotNull();

//            //RuleFor(x => x.Draws.Count).GreaterThan(0);
//            //RuleFor(x => x.NumBallDrawings.Count).GreaterThan(0);
//            //RuleFor(x => x.OddBalls.Length).GreaterThan(0);
//            //RuleFor(x => x.RenatoGianellaOccurrance.Count).GreaterThan(0);
//        }
//    }
//}
