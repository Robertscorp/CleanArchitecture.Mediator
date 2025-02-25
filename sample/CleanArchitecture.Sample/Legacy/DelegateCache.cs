using System.Collections.Concurrent;

namespace CleanArchitecture.Sample.Legacy
{

    internal static class DelegateCache
    {

        #region - - - - - - Fields - - - - - -

        private static readonly ConcurrentDictionary<Type, object?> s_ProviderFunctions = new();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public static Func<TInput, TOutput>? GetFunction<TInput, TOutput>(Type providerType)
        {
            if (!s_ProviderFunctions.TryGetValue(providerType, out var _Function))
            {
                _Function = (Activator.CreateInstance(providerType) as IDelegateProvider<TInput, TOutput>)?.GetFunction();
                _ = s_ProviderFunctions.TryAdd(providerType, _Function);
            }

            return _Function as Func<TInput, TOutput>;
        }

        #endregion Methods

    }

    internal interface IDelegateProvider<in TInput, out TOutput>
    {

        #region - - - - - - Methods - - - - - -

        Func<TInput, TOutput> GetFunction();

        #endregion Methods

    }

}
