namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Represents an input port to the pipeline.
    /// </summary>
    /// <typeparam name="TOutputPort">The type of output port.</typeparam>
    public interface IInputPort<out TOutputPort> { }

}
