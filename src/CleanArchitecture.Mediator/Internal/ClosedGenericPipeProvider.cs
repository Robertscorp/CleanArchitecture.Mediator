using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CleanArchitecture.Mediator.Internal
{

    internal interface IClosedGenericPipeProvider
    {

        #region - - - - - - Methods - - - - - -

        IPipe<TInputPort, TOutputPort> GetPipe<TInputPort, TOutputPort>(ServiceFactory serviceFactory) where TInputPort : IInputPort<TOutputPort>;

        #endregion Methods

    }

    internal class ClosedGenericPipeProvider : IClosedGenericPipeProvider
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<Type, Type, ServiceFactory, object> m_PipeProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ClosedGenericPipeProvider(Type openGenericPipeType, Type[] closedGenericTypes)
        {
            if (!openGenericPipeType.IsGenericTypeDefinition)
                throw new ArgumentException($"{nameof(openGenericPipeType)} is not an open generic type.");

            var _PipeInterfaceType = typeof(IPipe<,>);
            var _PipeInterfaceTypeID = _PipeInterfaceType.GUID;

            var _GenericPipeInterfaceTypes = openGenericPipeType.GetInterfaces().Where(i => i.GUID == _PipeInterfaceTypeID).ToList();
            if (_GenericPipeInterfaceTypes.Count != 1)
                throw new ArgumentException($"{nameof(openGenericPipeType)} must implement {_PipeInterfaceType.FullName} exactly once.");

            var _InputPortType = _GenericPipeInterfaceTypes[0].GenericTypeArguments[0];
            if (!_InputPortType.IsGenericParameter)
                throw new ArgumentException($"The input port specified on {_PipeInterfaceType.FullName} must be a generic parameter.");

            var _OutputPortType = _GenericPipeInterfaceTypes[0].GenericTypeArguments[1];
            if (!_OutputPortType.IsGenericParameter)
                throw new ArgumentException($"The output port specified on {_PipeInterfaceType.FullName} must be a generic parameter.");

            var _GenericPipeTypeArguments = openGenericPipeType.GetGenericArguments();
            if (_GenericPipeTypeArguments.Length != closedGenericTypes.Length + 2)
                throw new ArgumentException($"{nameof(closedGenericTypes)} must contain {_GenericPipeTypeArguments.Length - 2} type(s).");

            var _ClosedGenericIndex = 0;
            var _ClosedGenericTypesTemplate = new Type[_GenericPipeTypeArguments.Length];

            var _InputPortIndex = -1;
            var _OutputPortIndex = -1;

            for (var _Index = 0; _Index < _GenericPipeTypeArguments.Length; _Index++)
                if (_GenericPipeTypeArguments[_Index] == _InputPortType)
                    _InputPortIndex = _Index;

                else if (_GenericPipeTypeArguments[_Index] == _OutputPortType)
                    _OutputPortIndex = _Index;

                else
                    _ClosedGenericTypesTemplate[_Index] = closedGenericTypes[_ClosedGenericIndex++];

            var _PipeCache = new ConcurrentDictionary<Type, object>();

            this.m_PipeProvider = (inputPort, outputPort, serviceFactory)
                => _PipeCache.GetOrAdd(inputPort, _ =>
                {
                    var _TypeArray = (Type[])_ClosedGenericTypesTemplate.Clone();
                    _TypeArray[_InputPortIndex] = inputPort;
                    _TypeArray[_OutputPortIndex] = outputPort;

                    // Wrap the resolution of MakeGenericType in try-catch, in case TypeConstraintValidator returns a false positive.
                    try
                    {
                        if (TypeConstraintValidator.IsValid(openGenericPipeType, _TypeArray))
                            return serviceFactory(openGenericPipeType.MakeGenericType(_TypeArray));
                    }
                    catch { }

                    return null;
                });
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        IPipe<TInputPort, TOutputPort> IClosedGenericPipeProvider.GetPipe<TInputPort, TOutputPort>(ServiceFactory serviceFactory)
            => this.m_PipeProvider(typeof(TInputPort), typeof(TOutputPort), serviceFactory) as IPipe<TInputPort, TOutputPort>;

        #endregion Methods

    }

}
