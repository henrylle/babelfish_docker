using Poc.Web.Extensions;
using System;
using System.Threading.Tasks;

namespace Poc.Web.Command
{
    public abstract class HandlerBase
    {
        public Task<OperationResult<T>> CallFunction<T>(Func<T> function, Exception ex = null)
        {
            try
            {
                return Task.FromResult(OperationResult.Success(function.Invoke()));
            }
            catch (Exception e)
            {
                return ex == null ? Task.FromResult(OperationResult.Error<T>(e.InnerException ?? e)) : Task.FromResult(OperationResult.Error<T>(ex));
            }
        }
    }
}
