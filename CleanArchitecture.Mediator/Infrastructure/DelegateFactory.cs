using System;
using System.Collections.Concurrent;

namespace CleanArchitecture.Mediator.Infrastructure
{

    internal static class DelegateFactory
    {

        #region - - - - - - Fields - - - - - -

        private static ConcurrentDictionary<Type, object> s_FactoryFunctions
            = new ConcurrentDictionary<Type, object>();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public static Func<TInput, TOutput> GetFunction<TInput, TOutput>(Type factoryType)
        {
            if (!s_FactoryFunctions.TryGetValue(factoryType, out var _Function))
            {
                _Function = (Activator.CreateInstance(factoryType) as IDelegateFactory<TInput, TOutput>)?.GetFunction();
                _ = s_FactoryFunctions.TryAdd(factoryType, _Function);
            }

            return _Function as Func<TInput, TOutput>;
        }

        #endregion Methods

    }

    internal interface IDelegateFactory<in TInput, out TOutput>
    {

        #region - - - - - - Methods - - - - - -

        Func<TInput, TOutput> GetFunction();

        #endregion Methods

    }

}
