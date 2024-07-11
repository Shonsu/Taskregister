using FluentValidation;
using Taskregister.Server.Task.Controller.Dto;

namespace Taskregister.Server.Properties
{
    public class QueryParametersValidator : AbstractValidator<QueryParameters>
    {
        public QueryParametersValidator()
        {
        }
    }
}
