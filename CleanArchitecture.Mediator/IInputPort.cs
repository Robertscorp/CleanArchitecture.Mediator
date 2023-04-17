namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Marks a class as being an input port.
    /// </summary>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    public interface IInputPort<out TOutputPort> { }

}
