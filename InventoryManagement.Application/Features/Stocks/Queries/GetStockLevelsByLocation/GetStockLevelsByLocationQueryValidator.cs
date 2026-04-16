using FluentValidation;

namespace InventoryManagement.Application.Features.Stock.Queries.GetStockLevelsByLocation;

public class GetStockLevelsByLocationQueryValidator : AbstractValidator<GetStockLevelsByLocationQuery>
{
    public GetStockLevelsByLocationQueryValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID must be provided to query stock levels.");
    }
}