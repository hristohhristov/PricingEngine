namespace PricingEngine.Domain.Common;

/// <summary>
/// Marks an entity as soft-deletable, meaning it is hidden from queries rather than physically removed.
/// EF Core requires public setters on both properties so that the persistence layer can hydrate them.
/// </summary>
public interface ISoftDelete
{
    /// <summary>Gets or sets a value indicating whether the entity has been logically deleted.</summary>
    bool IsDeleted { get; set; }

    /// <summary>Gets or sets the UTC timestamp at which the entity was logically deleted; <c>null</c> if not deleted.</summary>
    DateTime? DeletedAt { get; set; }
}
