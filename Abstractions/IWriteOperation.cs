using PaymentGateway.Data;
using System;

namespace Abstractions
{
    public interface IWriteOperation<TCommand>
    {
        public void PerformOperation(TCommand operation);
    }
}
