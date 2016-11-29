namespace MassTransit.Testes.Specifications
{
    using GreenPipes;


    public interface ITestSubjectSpecification<TSubject> :
        ISpecification
        where TSubject : class, ITestSubject<TSubject>
    {
    }
}