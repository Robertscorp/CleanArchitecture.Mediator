namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Marks a class as being a Use Case's Input Port.
    /// </summary>
    /// <typeparam name="TOutputPort">The type of the Use Case's Output Port.</typeparam>
    public interface IUseCaseInputPort<out TOutputPort> { }

}
