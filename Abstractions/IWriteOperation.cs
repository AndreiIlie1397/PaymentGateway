//namespace Abstractions
//{
//    public interface IWriteOperation<TCommand>
//    {
//        public void PerformOperation(TCommand operation);
//    }
//}
namespace Abstractions
{
    public interface ITest<TCommand> // IRequest<TCommand>
    {
        void PerformOperation(TCommand operation);
    }
}