using System.Threading.Tasks;

namespace CleanArchitecture.Services.Pipeline
{

    /// <summary>
    /// A delegate that represents the HandleAsync method for an Element in the Use Case Pipeline.
    /// </summary>
    public delegate Task UseCaseElementHandleAsync();

}
