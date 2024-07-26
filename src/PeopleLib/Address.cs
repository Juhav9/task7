using System.ComponentModel.DataAnnotations;

namespace PeopleLib;

public class Address
{
    public int Id { get; set; }    

    [Required] 
    public string StreetAddress { get; set; } = null!;

    [Required] 
    public string PostalAddress { get; set; } = null!;
    
    [Required]
    public int PostalNumber { get; set; }
}
