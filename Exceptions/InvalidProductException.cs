namespace InventoryAudit.Exceptions;

/// <summary>Thrown when a product title is empty, too short, too long or contains no real characters.</summary>
public class InvalidProductException : Exception
{
    public InvalidProductException(string message) : base(message) { }
}

/// <summary>Thrown when a product with the same (cleaned) name already exists.</summary>
public class DuplicateProductException : InvalidProductException
{
    public DuplicateProductException(string name)
        : base($"A product named \"{name}\" already exists in the inventory.") { }
}
