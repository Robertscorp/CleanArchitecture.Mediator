using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Mediator
{

    /// <summary>
    /// Represents a continuation strategy that determines how the pipeline should proceed.
    /// </summary>
    public abstract class ContinuationBehaviour
    {

        #region - - - - - - Fields - - - - - -

        /// <summary>
        /// A continuation that invokes the next pipe in the pipeline.
        /// </summary>
        public static readonly ContinuationBehaviour Continue = new ContinueBehaviour();

        /// <summary>
        /// A continuation that does not invoke the next pipe in the pipeline.
        /// </summary>
        public static readonly ContinuationBehaviour Return = new ReturnBehaviour();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Creates a custom continuation that executes the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">A delegate that defines how the continuation behaves.</param>
        /// <returns>A continuation that delegates its behaviour to the specified <paramref name="handler"/>.</returns>
        public static ContinuationBehaviour Custom(Func<NextPipeHandleAsync, CancellationToken, Task> handler)
            => new CustomContinuationBehaviour(handler);

        /// <summary>
        /// Executes the continuation behaviour.
        /// </summary>
        /// <param name="nextPipeHandle">A delegate representing the next pipe in the pipeline.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
        public abstract Task HandleAsync(NextPipeHandleAsync nextPipeHandle, CancellationToken cancellationToken);

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        internal sealed class ContinueBehaviour : ContinuationBehaviour
        {

            #region - - - - - - Methods - - - - - -

            public override Task HandleAsync(NextPipeHandleAsync nextPipeHandle, CancellationToken cancellationToken)
                => nextPipeHandle();

            #endregion Methods

        }

        internal sealed class ReturnBehaviour : ContinuationBehaviour
        {

            #region - - - - - - Methods - - - - - -

            public override Task HandleAsync(NextPipeHandleAsync nextPipeHandle, CancellationToken cancellationToken)
                => Task.CompletedTask;

            #endregion Methods

        }

        internal sealed class CustomContinuationBehaviour : ContinuationBehaviour
        {

            #region - - - - - - Fields - - - - - -

            private readonly Func<NextPipeHandleAsync, CancellationToken, Task> m_HandleAsync;

            #endregion Fields

            #region - - - - - - Constructors - - - - - -

            public CustomContinuationBehaviour(Func<NextPipeHandleAsync, CancellationToken, Task> handleAsync)
                => this.m_HandleAsync = handleAsync ?? throw new ArgumentNullException(nameof(handleAsync));

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task HandleAsync(NextPipeHandleAsync nextPipeHandle, CancellationToken cancellationToken)
                => this.m_HandleAsync(nextPipeHandle, cancellationToken);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
