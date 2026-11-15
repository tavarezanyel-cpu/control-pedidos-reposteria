namespace BakeryOrderControl.Models;

/// <summary>Abstract base class: every entity has an Id and knows how to describe itself.</summary>
public abstract class BaseEntity
{
    public int Id { get; internal set; }

    public abstract string Describe();

    public override string ToString() => Describe();
}
