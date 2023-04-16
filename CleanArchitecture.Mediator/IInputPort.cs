namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Marks a class as being an Input Port.
    /// </summary>
    /// <typeparam name="TOutputPort">The type of Output Port.</typeparam>
    public interface IInputPort<out TOutputPort> { }

}
