using FluentValidation;
using OData.Models;

namespace OData.Validation
{
    public class PredictionEntityValidator : AbstractValidator<PredictionEntity>
        {
            public PredictionEntityValidator()
            {
                RuleFor(x => x.PartitionKey).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x.PartitionKey).Equal("Predictions");
                });
                RuleFor(x => x.RowKey).NotNull();
            }
        }
}
