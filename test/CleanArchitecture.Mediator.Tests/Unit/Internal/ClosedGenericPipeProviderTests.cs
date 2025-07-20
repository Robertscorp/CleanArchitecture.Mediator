using CleanArchitecture.Mediator.Internal;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal
{

    public class ClosedGenericPipeProviderTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly ServiceFactory m_ServiceFactory;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ClosedGenericPipeProviderTests()
        {
            var _MockServiceFactory = new Mock<ServiceFactory>();
            _ = _MockServiceFactory
                    .Setup(mock => mock(typeof(ConstrainedPipe<DisposableInputPort, IDisposable>)))
                    .Returns(new ConstrainedPipe<DisposableInputPort, IDisposable>());

            _ = _MockServiceFactory
                    .Setup(mock => mock(typeof(ValidPipe<object, IInputPort<DateTime>, string, DateTime, int>)))
                    .Returns(new ValidPipe<object, IInputPort<DateTime>, string, DateTime, int>());

            this.m_ServiceFactory = _MockServiceFactory.Object;
        }

        #endregion Constructors

        #region - - - - - - Constructor Tests - - - - - -

        [Fact]
        public void Constructor_PipeTypeIsNotAnOpenGeneric_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(NonGenericPipe), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_PipeTypeDoesNotImplementIPipeInterface_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(DoesNotImplementGenericIPipe<>), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_PipeTypeImplementsIPipeInterfaceMultipleTimes_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(ImplementsGenericIPipeTwice<>), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_IPipeInterfaceIsImplementedWithClosedGenericInputPort_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(ClosedGenericInputPort<>), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_IPipeInterfaceIsImplementedWithClosedGenericOutputPort_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(ClosedGenericOutputPort<>), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_IncorrectNumberOfClosedGenericTypesSpecified_FailsToConstruct()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(ValidPipe<,,,,>), Array.Empty<Type>())).Should().BeOfType<ArgumentException>();

        [Fact]
        public void Constructor_ValidSetup_ConstructsSuccessfully()
            => Record.Exception(() => new ClosedGenericPipeProvider(typeof(ValidPipe<,,,,>), new Type[] { typeof(object), typeof(object), typeof(object) })).Should().BeNull();

        #endregion Constructor Tests

        #region - - - - - - GetPipe Tests - - - - - -

        [Fact]
        public void GetPipe_GenericConstraintsAreNotMet_NoPipeReturned()
            => new ClosedGenericPipeProvider(typeof(ConstrainedPipe<,>), Array.Empty<Type>())
                .GetPipe<IInputPort<object>, object>(this.m_ServiceFactory)
                .Should()
                .BeNull();

        [Fact]
        public void GetPipe_ServiceFactoryDoesNotContainRegisteredPipe_NoPipeReturned()
            => new ClosedGenericPipeProvider(typeof(ConstrainedPipe<,>), Array.Empty<Type>())
                .GetPipe<DisposableInputPort2, IDisposable>(this.m_ServiceFactory)
                .Should()
                .BeNull();

        [Fact]
        public void GetPipe_GenericConstraintsAreMet_GetsPipeFromServiceFactory()
            => new ClosedGenericPipeProvider(typeof(ConstrainedPipe<,>), Array.Empty<Type>())
                    .GetPipe<DisposableInputPort, IDisposable>(this.m_ServiceFactory)
                    .Should()
                    .BeOfType<ConstrainedPipe<DisposableInputPort, IDisposable>>();

        [Fact]
        public void GetPipe_InputAndOutputPortParametersAreBetweenGenericParameters_ResolvesGenericsProperlyAndGetsPipeFromServiceFactory()
            => new ClosedGenericPipeProvider(typeof(ValidPipe<,,,,>), new Type[] { typeof(object), typeof(string), typeof(int) })
                .GetPipe<IInputPort<DateTime>, DateTime>(this.m_ServiceFactory)
                .Should()
                .BeOfType<ValidPipe<object, IInputPort<DateTime>, string, DateTime, int>>();

        #endregion GetPipe Tests

        #region - - - - - - Nested Classes - - - - - -

        private class ClosedGenericInputPort<T> : IPipe<IInputPort<T>, T>
        {
            Task IPipe<IInputPort<T>, T>.InvokeAsync(IInputPort<T> inputPort, T outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class ClosedGenericOutputPort<T> : IPipe<T, object> where T : IInputPort<object>
        {
            Task IPipe<T, object>.InvokeAsync(T inputPort, object outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class ConstrainedPipe<TInputPort, TOutputPort> : IPipe<TInputPort, TOutputPort>
            where TInputPort : IInputPort<TOutputPort>, IDisposable
            where TOutputPort : IDisposable
        {
            Task IPipe<TInputPort, TOutputPort>.InvokeAsync(TInputPort inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class DisposableInputPort : IInputPort<IDisposable>, IDisposable
        {
            void IDisposable.Dispose() => throw new NotImplementedException();
        }

        private class DisposableInputPort2 : IInputPort<IDisposable>, IDisposable
        {
            void IDisposable.Dispose() => throw new NotImplementedException();
        }

        private class DoesNotImplementGenericIPipe<T> { }

        private class ImplementsGenericIPipeTwice<T> : IPipe<IInputPort<object>, object>, IPipe<IInputPort<int>, int>
        {
            Task IPipe<IInputPort<object>, object>.InvokeAsync(IInputPort<object> inputPort, object outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
            Task IPipe<IInputPort<int>, int>.InvokeAsync(IInputPort<int> inputPort, int outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class NonGenericPipe : IPipe<IInputPort<object>, object>
        {
            Task IPipe<IInputPort<object>, object>.InvokeAsync(IInputPort<object> inputPort, object outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private class ValidPipe<T1, TInputPort, T2, TOutputPort, T3> : IPipe<TInputPort, TOutputPort>
            where TInputPort : IInputPort<TOutputPort>
        {
            Task IPipe<TInputPort, TOutputPort>.InvokeAsync(TInputPort inputPort, TOutputPort outputPort, ServiceFactory serviceFactory, IPipeHandle nextPipeHandle, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        #endregion Nested Classes

    }

}
