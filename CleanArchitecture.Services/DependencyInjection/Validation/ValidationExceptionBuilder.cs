using System.Text;

namespace CleanArchitecture.Services.DependencyInjection.Validation
{

    internal class ValidationExceptionBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<(Type InputPort, Type[] MissingServices)> m_MissingServices = new();
        private readonly List<(Type PipeOutputPort, Type[] AffectedUseCaseOutputPorts)> m_UnregisteredOutputPorts = new();

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        private bool HasValidationFailure
            => this.m_MissingServices.Any() ||
                this.m_UnregisteredOutputPorts.Any();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddMissingServices(Type inputPort, Type[] missingServices)
            => this.m_MissingServices.Add((inputPort, missingServices));

        public void AddUnregisteredOutputPort(Type pipeOutputPort, Type[] affectedUseCaseOutputPorts)
            => this.m_UnregisteredOutputPorts.Add((pipeOutputPort, affectedUseCaseOutputPorts));

        private string GetMessage()
        {
            var _StringBuilder = new StringBuilder(32);
            _ = _StringBuilder.AppendLine();

            foreach (var (_InputPort, _MissingServices) in this.m_MissingServices.OrderBy(ipms => ipms.InputPort.Name))
            {
                _ = _StringBuilder.Append(_InputPort.GetFriendlyName()).AppendLine(" is missing implementations for:");

                foreach (var _MissingService in _MissingServices.OrderBy(s => s.Name))
                    _ = _StringBuilder.Append(" - ").AppendLine(_MissingService.GetFriendlyName());

                _ = _StringBuilder.AppendLine();
            }

            foreach (var (_PipeOutputPort, _AffectedUseCaseOutputPorts) in this.m_UnregisteredOutputPorts.OrderBy(op => op.PipeOutputPort.Name))
            {
                _ = _StringBuilder
                        .Append(_PipeOutputPort.GetFriendlyName())
                        .AppendLine(" is not registered with the Use Case Pipeline. This affects:");

                foreach (var _AffectedUseCaseOutputPort in _AffectedUseCaseOutputPorts.OrderBy(op => op.GetFriendlyName()))
                    _ = _StringBuilder.Append(" - ").AppendLine(_AffectedUseCaseOutputPort.GetFriendlyName());

                _ = _StringBuilder.AppendLine();
            }

            return _StringBuilder.ToString().TrimEnd();
        }

        public ValidationException? ToValidationException()
            => this.HasValidationFailure ? new ValidationException(this.GetMessage()) : null;

        #endregion Methods

    }

}
