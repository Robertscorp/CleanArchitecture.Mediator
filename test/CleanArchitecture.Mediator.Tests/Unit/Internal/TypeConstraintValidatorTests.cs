using CleanArchitecture.Mediator.Internal;
using System;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit.Internal;

public class TypeConstraintValidatorTests
{

    #region - - - - - - IsValid Tests - - - - - -

    [Theory]
    [InlineData(typeof(AuthorisationPolicyValidationPipe<,,>), new Type[] { typeof(CreateEntityInputPort), typeof(ICreateEntityOutputPort), typeof(object) })]
    [InlineData(typeof(DoubleInterface<,>), new Type[] { typeof(DerivedClass), typeof(MoreDerivedClass) })]
    [InlineData(typeof(DoubleInterface<,>), new Type[] { typeof(DerivedClass), typeof(BaseClass) })]
    [InlineData(typeof(DoubleInterface<,>), new Type[] { typeof(MoreDerivedClass), typeof(MoreDerivedClass) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(DerivedClass), typeof(IDerivedClassConstraint<DerivedClass>) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(MoreDerivedClass), typeof(IDerivedClassConstraint<MoreDerivedClass>) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(MoreDerivedClass), typeof(IMoreDerivedClassConstraint<DerivedClass>) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(MoreDerivedClass), typeof(BaseClassConstraint<DerivedClass>) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(MoreDerivedClass), typeof(DerivedClassConstraint<DerivedClass>) })]
    [InlineData(typeof(DoubleInterface2<,>), new Type[] { typeof(MoreDerivedClass), typeof(MoreDerivedClassConstraint<DerivedClass>) })]
    [InlineData(typeof(IB<,>), new Type[] { typeof(IA<BaseClass>), typeof(object) })]
    [InlineData(typeof(IB<,>), new Type[] { typeof(IA<BaseClass>), typeof(BaseClass) })]
    [InlineData(typeof(IC<,>), new Type[] { typeof(object), typeof(object) })]
    [InlineData(typeof(IC<,>), new Type[] { typeof(IB<IA<BaseClass>, BaseClass>), typeof(BaseClass) })]
    [InlineData(typeof(INew<>), new Type[] { typeof(object) })]
    [InlineData(typeof(INew<>), new Type[] { typeof(NoDefaultConstructor) })]
    [InlineData(typeof(IClass<>), new Type[] { typeof(object) })]
    [InlineData(typeof(IClass<>), new Type[] { typeof(int) })]
    [InlineData(typeof(IStruct<>), new Type[] { typeof(int) })]
    [InlineData(typeof(IStruct<>), new Type[] { typeof(int?) })]
    [InlineData(typeof(IStruct<>), new Type[] { typeof(object) })]
    [InlineData(typeof(IStructWrapper<,>), new Type[] { typeof(IStruct<int>), typeof(int) })]
    [InlineData(typeof(IStructWrapper<,>), new Type[] { typeof(IStruct<int>), typeof(object) })]
    [InlineData(typeof(IStructWrapper<,>), new Type[] { typeof(IStruct<>), typeof(int) })]
    [InlineData(typeof(IStructWrapper2<,>), new Type[] { typeof(int), typeof(IStruct<int>) })]
    [InlineData(typeof(IStructWrapper2<,>), new Type[] { typeof(decimal), typeof(IStruct<decimal>) })]
    [InlineData(typeof(IStructWrapper2<,>), new Type[] { typeof(int?), typeof(IStruct<int>) })]
    [InlineData(typeof(IStructWrapper2<,>), new Type[] { typeof(decimal?), typeof(IStruct<decimal>) })]
    [InlineData(typeof(IStructWrapper2<,>), new Type[] { typeof(object), typeof(IStruct<int>) })]
    public void IsValid_VariousTypes_IsValidMatchesMakeGenericType(Type openGenericType, Type[] closedGenericTypes)
        => Assert.Equal(
            TypeConstraintValidator.IsValid(openGenericType, closedGenericTypes),
            Record.Exception(() => openGenericType.MakeGenericType(closedGenericTypes)) == null);

    #endregion IsValid Tests

    #region - - - - - - Nested Classes - - - - - -

    private class CreateEntityInputPort : IInputPort<ICreateEntityOutputPort> { }

    private interface ICreateEntityOutputPort : IAuthorisationPolicyFailureOutputPort<string>, IInputPortValidationFailureOutputPort<object> { }

    private class BaseClass { }

    private class DerivedClass : BaseClass { }

    private class MoreDerivedClass : DerivedClass { }

    private interface IBaseClassConstraint<out T> where T : BaseClass { }

    private class BaseClassConstraint<T> : IBaseClassConstraint<T> where T : BaseClass { }

    private interface IDerivedClassConstraint<T> where T : DerivedClass { }

    private class DerivedClassConstraint<T> : IDerivedClassConstraint<T> where T : DerivedClass { }

    private interface IMoreDerivedClassConstraint<T> : IDerivedClassConstraint<T> where T : DerivedClass { }

    private class MoreDerivedClassConstraint<T> : IMoreDerivedClassConstraint<T> where T : DerivedClass { }

    private interface IDoubleInterface<T1, T2, TDerived>
        where T1 : IBaseClassConstraint<BaseClass>
        where T2 : IDerivedClassConstraint<TDerived>
        where TDerived : DerivedClass
    { }

    private interface IDoubleInterface2<T1, out T2>
        where T1 : IBaseClassConstraint<BaseClass>
        where T2 : IDerivedClassConstraint<DerivedClass>
    { }

    private class DoubleInterface<T1, T2> : IDoubleInterface<IBaseClassConstraint<T1>, IDerivedClassConstraint<T2>, T2>
        where T1 : BaseClass
        where T2 : DerivedClass
    { }

    private class DoubleInterface2<T1, T2> : IDoubleInterface2<IBaseClassConstraint<T1>, T2>
        where T1 : BaseClass
        where T2 : IDerivedClassConstraint<DerivedClass>
    { }

    private interface IA<T> where T : BaseClass { }

    private interface IB<T1, T2> where T1 : IA<T2> where T2 : BaseClass { }

    private interface IC<T1, T2> where T1 : IB<IA<T2>, T2> where T2 : BaseClass { }

    private interface INew<T> where T : new() { }

    private interface IClass<T> where T : class { }

    private interface IStruct<T> where T : struct { }

    private interface IStructWrapper<T1, T2> where T1 : IStruct<T2> where T2 : struct { }

    private interface IStructWrapper2<T1, T2> where T1 : struct where T2 : IStruct<T1> { }

#pragma warning disable CS9113 // Parameter is unread.
    private class NoDefaultConstructor(object o) { }
#pragma warning restore CS9113 // Parameter is unread.

    #endregion Nested Classes

}
