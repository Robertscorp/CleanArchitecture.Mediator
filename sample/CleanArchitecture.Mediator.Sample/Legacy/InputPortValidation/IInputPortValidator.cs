namespace CleanArchitecture.Mediator.Sample.Legacy.InputPortValidation;

/// <summary>
/// A service used to determine if it's valid for the pipeline to continue.
/// </summary>
/// <typeparam name="TInputPort">The type of input port.</typeparam>
/// <typeparam name="TValidationResult">The type of validation result for the pipeline.</typeparam>
public interface IInputPortValidator<TInputPort, TValidationResult>
    where TInputPort : IInputPort<IInputPortValidationOutputPort<TValidationResult>>
    where TValidationResult : IInputPortValidationResult
{

    #region - - - - - - Methods - - - - - -

    /// <summary>
    /// Determines if it's valid for the pipeline to coninue.
    /// </summary>
    /// <param name="inputPort">The input to the pipeline.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be cancelled.</param>
    /// <returns>A validation result indicating if it's valid for the pipeline to continue.</returns>
    Task<TValidationResult> ValidateAsync(TInputPort inputPort, CancellationToken cancellationToken);

    #endregion Methods

}
