using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Mediator.Tests.Unit;

public class ContinuationBehaviourTests
{

    #region - - - - - - Aggregate Tests - - - - - -

    [Fact]
    public void Aggregate_MultipleContinuationBehaviours_FirstWithHighestSeverityChosen()
    {
        // Arrange
        var _Expected = ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10);

        // Act
        var _Actual
            = ContinuationBehaviour.Aggregate(
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 1),
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 5),
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 9),
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 6),
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 3),
                _Expected,
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10),
                ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10));

        // Assert
        _ = _Actual.Should().Be(_Expected);
    }

    #endregion Aggregate Tests

    #region - - - - - - AggregateWith Tests - - - - - -

    [Fact]
    public void AggregateWith_ContinuationBehaviourParameterHasLowerPriority_ThisInstanceChosen()
    {
        // Arrange
        var _Expected = ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10);

        // Act
        var _Actual = _Expected.AggregateWith(ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 9));

        // Assert
        _ = _Actual.Should().Be(_Expected);
    }

    [Fact]
    public void AggregateWith_ContinuationBehaviourParameterHasSamePriority_ThisInstanceChosen()
    {
        // Arrange
        var _Expected = ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10);

        // Act
        var _Actual = _Expected.AggregateWith(ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10));

        // Assert
        _ = _Actual.Should().Be(_Expected);
    }

    [Fact]
    public void AggregateWith_ContinuationBehaviourParameterHasHigherPriority_ThisInstanceChosen()
    {
        // Arrange
        var _Expected = ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 11);

        // Act
        var _Actual = ContinuationBehaviour.Custom((_, _) => Task.CompletedTask, 10).AggregateWith(_Expected);

        // Assert
        _ = _Actual.Should().Be(_Expected);
    }

    #endregion AggregateWith Tests

}
