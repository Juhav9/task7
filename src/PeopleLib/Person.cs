using System.ComponentModel.DataAnnotations;

namespace PeopleLib;

public class Person
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;
}
