using FluentValidation;
using System;
using Domain.Models;

namespace Domain.Validation
{
    public class ThunderballEntityValidator : AbstractValidator<ThunderBallEntity>
    {
		public ThunderballEntityValidator()
		{
            RuleFor(x => x.PartitionKey).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PartitionKey).Equal("Thunderball");
            });
			RuleFor(x => x.RowKey).NotNull();

            RuleSet("NewEntry", () =>
            {
                RuleFor(x => x.PartitionKey).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x.PartitionKey).Equal("Thunderball");
                });
                RuleFor(x => x.RowKey).NotNull();
                RuleFor(x => x.Ball1).NotNull();
                RuleFor(x => x.Ball2).NotNull();
                RuleFor(x => x.Ball3).NotNull();
                RuleFor(x => x.Ball4).NotNull();
                RuleFor(x => x.Ball5).NotNull();
                RuleFor(x => x.Thunderball).NotNull();
                RuleFor(x => x.Machine).NotNull();
                RuleFor(x => x.DrawNumber).NotNull();
                RuleFor(x => x.DrawDate).NotNull().DependentRules(() =>
                {
                    RuleFor(x => DateTime.Parse(x.DrawDate)).GreaterThanOrEqualTo(DateTime.Today.AddYears(-2));
                });
            });

        }

	}
}
