namespace CleanArchitecture.Mediator.Sample.Domain.Entities;

public class SampleEntity
{

    #region - - - - - - Properties - - - - - -

    public int CategoryID { get; set; } // This should be a navigation instead.

    public required string Name { get; set; }

    #endregion Properties

}
