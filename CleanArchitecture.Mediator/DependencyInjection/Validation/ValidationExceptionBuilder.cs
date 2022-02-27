using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanArchitecture.Mediator.DependencyInjection.Validation
{

    internal class ValidationExceptionBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<Type> m_MissingSingleImplementationServices = new List<Type>();
        private readonly List<(Type InputPort, Type[] MissingServices)> m_MissingUseCaseServices = new List<(Type InputPort, Type[] MissingServices)>();
        private readonly List<(Type PipeOutputPort, Type[] AffectedUseCaseOutputPorts)> m_UnregisteredOutputPorts = new List<(Type PipeOutputPort, Type[] AffectedUseCaseOutputPorts)>();

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        private bool HasValidationFailure
            => this.m_MissingSingleImplementationServices.Any() ||
                this.m_MissingUseCaseServices.Any() ||
                this.m_UnregisteredOutputPorts.Any();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddMissingSingleImplementationServices(Type[] missingSingleImplementationServices)
            => this.m_MissingSingleImplementationServices.AddRange(missingSingleImplementationServices);

        public void AddMissingUseCaseServices(Type inputPort, Type[] missingUseCaseServices)
            => this.m_MissingUseCaseServices.Add((inputPort, missingUseCaseServices));

        public void AddUnregisteredOutputPort(Type pipeOutputPort, Type[] affectedUseCaseOutputPorts)
            => this.m_UnregisteredOutputPorts.Add((pipeOutputPort, affectedUseCaseOutputPorts));

        private string GetMessage()
        {
            var _StringBuilder = new StringBuilder(32);
            _ = _StringBuilder.AppendLine();

            foreach (var _MissingService in this.m_MissingSingleImplementationServices)
                _ = _StringBuilder.Append(_MissingService.GetFriendlyName()).AppendLine(" has not been implemented.");

            if (this.m_MissingSingleImplementationServices.Any())
                _ = _StringBuilder.AppendLine();

            foreach (var (_InputPort, _MissingServices) in this.m_MissingUseCaseServices.OrderBy(ipms => ipms.InputPort.Name))
            {
                _ = _StringBuilder.Append(_InputPort.GetFriendlyName()).AppendLine(" is missing implementations for:");

                foreach (var _MissingService in _MissingServices.OrderBy(s => s.GetFriendlyName()))
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

        public ValidationException ToValidationException()
            => this.HasValidationFailure ? new ValidationException(this.GetMessage()) : null;

        #endregion Methods

    }

}
