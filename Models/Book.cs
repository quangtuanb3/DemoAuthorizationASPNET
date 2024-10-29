using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public string Author { get; set; } = default!;

    public string? Genre { get; set; }

    public int YearPublished { get; set; }

}