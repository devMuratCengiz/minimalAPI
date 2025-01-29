using System.ComponentModel.DataAnnotations;

namespace ch_08_di.Entities.DTOs
{
    public abstract record BookDto
    {
        [MinLength(2, ErrorMessage = "Min len. must be 2")]
        [MaxLength(20, ErrorMessage = "Max len. must be 20")]
        public string Title { get; init; }

        [Range(10, 100, ErrorMessage ="Price must be between 10-100")]
        public decimal Price { get; set; }
    }
}
