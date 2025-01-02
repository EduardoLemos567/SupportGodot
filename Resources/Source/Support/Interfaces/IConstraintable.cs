namespace Support;

/// <summary>
/// Objects that have a certain constraint that need to be checked 
/// and readjusted to work properly. A specie of sanity check and fix.
/// </summary>
public interface IConstraintable
{
    void EnforceConstraint();
}