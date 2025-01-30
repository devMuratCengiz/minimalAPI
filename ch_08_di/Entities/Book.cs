using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }
    
    public string? Title { get; set; }
    public decimal Price { get; set; }
    public string Url { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } //navigation property
    public Book()
    {
        Url = "/images/default.jpg";
    }

}
