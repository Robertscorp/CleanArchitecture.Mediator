using System;
using System.Collections.Concurrent;

namespace CleanArchitecture.Services.Infrastructure
{

    internal static class DelegateFactory
    {

        #region - - - - - - Fields - - - - - -

        private static ConcurrentDictionary<Type, object> s_Factories
            = new ConcurrentDictionary<Type, object>();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public static Func<TInput, TOutput> GetFunction<TInput, TOutput>(Type factoryType, UseCaseServiceResolver serviceResolver)
        {
            if (!s_Factories.TryGetValue(factoryType, out var _Factory))
            {
                _Factory = (IDelegateFactory<TInput, TOutput>)Activator.CreateInstance(factoryType);
                _ = s_Factories.TryAdd(factoryType, _Factory);
            }

            return (_Factory as IDelegateFactory<TInput, TOutput>)?.GetFunction(serviceResolver);
        }

        #endregion Methods

    }

    internal interface IDelegateFactory<in TInput, out TOutput>
    {

        #region - - - - - - Methods - - - - - -

        Func<TInput, TOutput> GetFunction(UseCaseServiceResolver serviceResolver);

        #endregion Methods

    }

}
