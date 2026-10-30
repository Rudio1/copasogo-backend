namespace Copasogo.Domain.Generic.Entities;

public abstract class Entity : IEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    protected void MarkAsUpdated(Guid? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        if (updatedBy.HasValue)
        {
            UpdatedBy = updatedBy;
        }
    }

    public void SetCreatedAudit(Guid? createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetUpdatedAudit(Guid? updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Delete(Guid? deletedBy = null)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        MarkAsUpdated(deletedBy);
    }

    public void Restore(Guid? restoredBy = null)
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        MarkAsUpdated(restoredBy);
    }
}
