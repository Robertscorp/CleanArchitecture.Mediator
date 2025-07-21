namespace CleanArchitecture.Mediator.Sample.UseCases.LegacyCreateProduct;

public class LegacyCreateProductInteractor : IInteractor<LegacyCreateProductInputPort, ILegacyCreateProductOutputPort>
{

    #region - - - - - - Methods - - - - - -

    Task IInteractor<LegacyCreateProductInputPort, ILegacyCreateProductOutputPort>.HandleAsync(
        LegacyCreateProductInputPort inputPort,
        ILegacyCreateProductOutputPort outputPort,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
        => outputPort.PresentCreatedProductAsync(new() { Name = $"Created - {DateTime.Now}" }, cancellationToken);

    #endregion Methods

}
