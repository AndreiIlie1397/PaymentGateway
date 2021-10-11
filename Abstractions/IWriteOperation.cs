using PaymentGateway.Data;
using System;

namespace Abstractions
{
    public interface IWriteOperation<T>
    {
        public void PerformOperation(T operation, Database database);
    }
}
