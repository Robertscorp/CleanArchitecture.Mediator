using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Invokes the next pipe in the pipeline.
    /// </summary>
    /// <remarks>
    /// The pipe is invoked with the same input port, output port, <see cref="ServiceFactory"/>, and <see cref="CancellationToken"/> passed into the pipeline.
    /// </remarks>
    public delegate Task NextPipeHandleAsync();

}
