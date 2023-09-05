using Domain.Models;
using FluentValidation;

namespace Domain.Validation
{
    public class NeuLotteryValidator : AbstractValidator<NeuLotteryEntity>
    {
        public NeuLotteryValidator()
        {
            RuleFor(x => x.PartitionKey).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PartitionKey == x.Name);
            });
            RuleFor(x => x.RowKey).NotNull();

            RuleSet("NewEntry", () =>
            {
                RuleFor(x => x.PartitionKey).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x.PartitionKey == x.Name);
                });

                RuleFor(x => x.RowKey).NotNull().NotNull();
                RuleFor(x => x.Draw).NotNull();
            });

        }
    }
}
