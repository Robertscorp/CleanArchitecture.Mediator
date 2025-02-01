using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Invokes the next pipe in the pipeline.
    /// </summary>
    public delegate Task NextPipeHandleAsync();

}
