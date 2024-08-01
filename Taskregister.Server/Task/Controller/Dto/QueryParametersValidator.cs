using FluentValidation;

namespace Taskregister.Server.Task.Controller.Dto
{
    public class QueryParametersValidator : AbstractValidator<QueryParameters>
    {
        public QueryParametersValidator()
        {
            // TODO check priority types,
            // TODO dateFrom should be before dateTo
        }
    }
}
