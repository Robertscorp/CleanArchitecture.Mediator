using System;

namespace CleanArchitecture.Mediator.Setup
{

    /// <summary>
    /// Represents a mutually exclusive authentication configuration for the pipeline.
    /// </summary>
    /// <remarks>
    /// Attempting to register both <see cref="MultiPrincipal"/> and <see cref="SinglePrincipal"/> will result in an <see cref="InvalidOperationException"/>.
    /// </remarks>

    public class AuthenticationMode
    {

        #region - - - - - - Fields - - - - - -

        /// <summary>
        /// Configures authentication to support multiple concurrently authenticated principals.
        /// </summary>
        public static readonly AuthenticationMode MultiPrincipal;

        /// <summary>
        /// Configures authentication to only support a single authenticated principal.
        /// </summary>
        public static readonly AuthenticationMode SinglePrincipal;

        private readonly Action<PackageRegistration> m_RegisterServiceAction;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        private AuthenticationMode(Action<PackageRegistration> registerServiceAction)
            => this.m_RegisterServiceAction = registerServiceAction;

        static AuthenticationMode()
        {
            var _MultiPrincipalRegistered = false;
            var _SinglePrincipalRegistered = false;

            MultiPrincipal = new AuthenticationMode(packageRegistration =>
            {
                if (_SinglePrincipalRegistered)
                    throw new InvalidOperationException("Cannot register both single principal and multi principal authentication.");

                _ = packageRegistration.AddScopedService(typeof(IPrincipalAccessor));

                _MultiPrincipalRegistered = true;
            });

            SinglePrincipal = new AuthenticationMode(packageRegistration =>
            {
                if (_MultiPrincipalRegistered)
                    throw new InvalidOperationException("Cannot register both single principal and multi principal authentication.");

                _ = packageRegistration.AddSingletonService(typeof(IPrincipalAccessor));

                _SinglePrincipalRegistered = true;
            });
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        internal void RegisterService(PackageRegistration packageRegistration)
            => this.m_RegisterServiceAction(packageRegistration);

        #endregion Methods

    }

}
