using System;
using System.Reflection;

namespace CleanArchitecture.Mediator.Internal
{

    internal static class TypeConstraintValidator
    {

        #region - - - - - - Fields - - - - - -

        private static Guid s_NullableTypeID = typeof(int?).GUID;

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public static bool IsValid(Type openGenericType, Type[] closedGenericTypes)
        {
            if (!openGenericType.IsGenericTypeDefinition)
                return false;

            var _OpenGenericParameters = openGenericType.GetGenericArguments();
            if (_OpenGenericParameters.Length != closedGenericTypes.Length)
                return false;

            for (var _OpenGenericParameterIndex = 0; _OpenGenericParameterIndex < _OpenGenericParameters.Length; _OpenGenericParameterIndex++)
            {
                var _OpenGenericParameter = _OpenGenericParameters[_OpenGenericParameterIndex];
                var _GenericParameterAttributes = _OpenGenericParameter.GenericParameterAttributes;

                var _ClosedGenericType = closedGenericTypes[_OpenGenericParameterIndex];
                if (_ClosedGenericType.IsValueType)
                {
                    if (_GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                        return false;

                    if (_ClosedGenericType.GUID == s_NullableTypeID && _GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                        return false;
                }
                else if (_GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                    return false;

                else if (_GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) && _ClosedGenericType.GetConstructor(Type.EmptyTypes) == null)
                    return false;

                var _OpenGenericParameterConstraints = _OpenGenericParameter.GetGenericParameterConstraints();
                for (var _ConstraintIndex = 0; _ConstraintIndex < _OpenGenericParameterConstraints.Length; _ConstraintIndex++)
                    if (!IsValid(_OpenGenericParameterConstraints[_ConstraintIndex], _ClosedGenericType, _OpenGenericParameters, closedGenericTypes))
                        return false;
            }

            return true;
        }

        private static bool IsValid(Type constraint, Type closedGenericType, Type[] openGenericParameters, Type[] closedGenericTypes)
        {
            var _Constraint = ResolveConstraint(constraint, openGenericParameters, closedGenericTypes);
            return _Constraint != null && _Constraint.IsAssignableFrom(closedGenericType);
        }

        private static Type ResolveConstraint(Type constraint, Type[] openGenericParameters, Type[] closedGenericTypes)
        {
            if (constraint.IsGenericParameter)
                return closedGenericTypes[Array.IndexOf(openGenericParameters, constraint)];

            if (constraint.IsGenericType)
            {
                var _ConstraintArguments = constraint.GetGenericArguments();

                for (var _Index = 0; _Index < _ConstraintArguments.Length; _Index++)
                {
                    var _Constraint = _ConstraintArguments[_Index];
                    if (_Constraint.IsGenericParameter)
                    {
                        var _InnerConstraints = _Constraint.GetGenericParameterConstraints();
                        for (var _InnerIndex = 0; _InnerIndex < _InnerConstraints.Length; _InnerIndex++)
                            if (!IsValid(_InnerConstraints[_InnerIndex], closedGenericTypes[Array.IndexOf(openGenericParameters, _Constraint)], openGenericParameters, closedGenericTypes))
                                return null;
                    }

                    _ConstraintArguments[_Index] = ResolveConstraint(_Constraint, openGenericParameters, closedGenericTypes);
                }

                return constraint.GetGenericTypeDefinition().MakeGenericType(_ConstraintArguments);
            }

            return constraint;
        }

        #endregion Methods

    }

}
