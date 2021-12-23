using System.Text;

namespace CleanArchitecture.Services.DependencyInjection.Validation
{

    internal class ValidationExceptionBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<(Type InputPort, Type[] MissingServices)> m_MissingServices = new();

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        private bool HasValidationFailure => this.m_MissingServices.Any();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddMissingServices(Type inputPort, Type[] missingServices)
            => this.m_MissingServices.Add((inputPort, missingServices));

        private string GetMessage()
        {
            var _StringBuilder = new StringBuilder(32);
            _ = _StringBuilder.AppendLine();

            if (this.m_MissingServices.Any())
            {
                foreach (var (_InputPort, _MissingServices) in this.m_MissingServices.OrderBy(ipms => ipms.InputPort.Name))
                {
                    _ = _StringBuilder.Append(_InputPort.GetFriendlyName()).AppendLine(" is missing implementations for:");

                    foreach (var _MissingService in _MissingServices.OrderBy(s => s.Name))
                        _ = _StringBuilder.Append(" - ").AppendLine(_MissingService.GetFriendlyName());
                    _ = _StringBuilder.AppendLine();
                }
            }

            return _StringBuilder.ToString().TrimEnd();
        }

        public ValidationException? ToValidationException()
            => this.HasValidationFailure ? new ValidationException(this.GetMessage()) : null;

        #endregion Methods

    }

}
