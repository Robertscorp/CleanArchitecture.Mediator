namespace CleanArchitecture.Services.Infrastructure
{

    internal delegate Task<TResult>? InvokeAsync<TParameter, TResult>(TParameter parameter, CancellationToken cancellationToken);

    internal abstract class InvokeAsyncFactory<TParameter, TResult>
    {

        #region - - - - - - Methods - - - - - -

        public abstract InvokeAsync<TParameter, TResult> GetInvokeAsync(UseCaseServiceResolver serviceResolver);

        public static Task<TResult>? InvokeFactoryAsync(
            Type factoryType,
            UseCaseServiceResolver serviceResolver,
            TParameter parameter,
            CancellationToken cancellationToken)
            => Activator.CreateInstance(factoryType) is InvokeAsyncFactory<TParameter, TResult> _Factory
                ? _Factory.GetInvokeAsync(serviceResolver).Invoke(parameter, cancellationToken)
                : default;

        #endregion Methods

    }

}
