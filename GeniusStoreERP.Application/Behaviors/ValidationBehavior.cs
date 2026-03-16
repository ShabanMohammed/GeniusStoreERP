using FluentValidation;
using MediatR;

namespace GeniusStoreERP.Application.Behaviors;

public class ValidationBehavior<IRequest, Response>(IEnumerable<IValidator<IRequest>> validators)
        : IPipelineBehavior<IRequest, Response> where IRequest : notnull
{
    public async Task<Response> Handle(IRequest request, RequestHandlerDelegate<Response> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<IRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        if (failures.Count != 0)
            throw new ValidationException(failures);
        return await next(cancellationToken);
    }
}
