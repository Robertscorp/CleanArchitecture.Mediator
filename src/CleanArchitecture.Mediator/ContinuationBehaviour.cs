using System;
using System.Linq;
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
        /// A continuation that invokes the next pipe in the pipeline. Severity 0.
        /// </summary>
        public static readonly ContinuationBehaviour Continue = new ContinueBehaviour(severity: 0);

        /// <summary>
        /// A continuation that invokes the next pipe in the pipeline. Severity 0.
        /// </summary>
        public static readonly Task<ContinuationBehaviour> ContinueAsync = Task.FromResult(Continue);

        /// <summary>
        /// A continuation that does not invoke the next pipe in the pipeline. Severity 10.
        /// </summary>
        public static readonly ContinuationBehaviour Return = new ReturnBehaviour(severity: 10);

        /// <summary>
        /// A continuation that does not invoke the next pipe in the pipeline. Severity 10.
        /// </summary>
        public static readonly Task<ContinuationBehaviour> ReturnAsync = Task.FromResult(Return);

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="ContinuationBehaviour"/> class with the specified severity.
        /// </summary>
        /// <param name="severity">Determines how severe this <see cref="ContinuationBehaviour"/> is relative to other continuation behaviours.</param>
        protected ContinuationBehaviour(int severity)
            => this.Severity = severity;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines how severe this <see cref="ContinuationBehaviour"/> is relative to other continuation behaviours.
        /// </summary>
        /// <remarks>
        /// Used to determine which continuation behaviour to choose when aggregating multiple continuation behaviours.
        /// </remarks>
        public int Severity { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Aggregates the specified <paramref name="continuationBehaviours"/> into a single <see cref="ContinuationBehaviour"/>.
        /// </summary>
        /// <param name="continuationBehaviours">A collection of continuation behaviours to be aggregated.</param>
        /// <returns>The highest <see cref="Severity"/> <see cref="ContinuationBehaviour"/> in the specified <paramref name="continuationBehaviours"/> collection.</returns>
        /// <remarks>
        /// If multiple continuation behaviours have the same severity, the first <see cref="ContinuationBehaviour"/> of that severity will be chosen.
        /// </remarks>
        public static ContinuationBehaviour Aggregate(params ContinuationBehaviour[] continuationBehaviours)
            => continuationBehaviours.Aggregate((aggregate, increment) => aggregate.AggregateWith(increment));

        /// <summary>
        /// Aggregates the specified <paramref name="continuationBehaviour"/> with this <see cref="ContinuationBehaviour"/>.
        /// </summary>
        /// <param name="continuationBehaviour">A <see cref="ContinuationBehaviour"/> to aggregate with this <see cref="ContinuationBehaviour"/>.</param>
        /// <returns>
        /// The highest <see cref="Severity"/> <see cref="ContinuationBehaviour"/> being aggregated.
        /// If both continuation behaviours have the same severity, this <see cref="ContinuationBehaviour"/> will be returned.
        /// </returns>
        public ContinuationBehaviour AggregateWith(ContinuationBehaviour continuationBehaviour)
            => continuationBehaviour.Severity > this.Severity ? continuationBehaviour : this;

        /// <summary>
        /// Creates a custom continuation that executes the specified <paramref name="handler"/>. Severity must be specified.
        /// </summary>
        /// <param name="handler">A delegate that defines how the continuation behaves.</param>
        /// <param name="severity">Determines how severe this <see cref="ContinuationBehaviour"/> is relative to other continuation behaviours.</param>
        /// <returns>A continuation that delegates its behaviour to the specified <paramref name="handler"/>.</returns>
        public static ContinuationBehaviour Custom(Func<NextPipeHandleAsync, CancellationToken, Task> handler, int severity)
            => new CustomContinuationBehaviour(handler, severity);

        /// <summary>
        /// Creates a custom continuation that executes the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">A delegate that defines how the continuation behaves.</param>
        /// <param name="severity">Determines how severe this <see cref="ContinuationBehaviour"/> is relative to other continuation behaviours.</param>
        /// <returns>A continuation that delegates its behaviour to the specified <paramref name="handler"/>.</returns>
        public static Task<ContinuationBehaviour> CustomAsync(Func<NextPipeHandleAsync, CancellationToken, Task> handler, int severity)
            => Task.FromResult(Custom(handler, severity));

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

            #region - - - - - - Constructors - - - - - -

            public ContinueBehaviour(int severity) : base(severity) { }

            #endregion Constructors

            #region - - - - - - Methods - - - - - -

            public override Task HandleAsync(NextPipeHandleAsync nextPipeHandle, CancellationToken cancellationToken)
                => nextPipeHandle();

            #endregion Methods

        }

        internal sealed class ReturnBehaviour : ContinuationBehaviour
        {

            #region - - - - - - Constructors - - - - - -

            public ReturnBehaviour(int severity) : base(severity) { }

            #endregion Constructors

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

            public CustomContinuationBehaviour(Func<NextPipeHandleAsync, CancellationToken, Task> handleAsync, int severity) : base(severity)
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
