using System.Threading.Tasks;

namespace Support.Auth.Id.Common.CommandQuery;

public interface ICommandQueryHandler<in TQuery, TResult> where TQuery : ICommandQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
