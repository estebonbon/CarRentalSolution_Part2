namespace CarRental.Models
{
    internal interface IAuditable
    {
        string? CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string? UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }

    //This is part of Part2C, and needs to be done before creating the Auditable Class, that will inherit from this interface.
}
