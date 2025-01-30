namespace ch_08_di.Entities.DTOs.Book
{
    public record BookDto : BookDtoBase
    {
        public int Id { get; init; }
        public Category Category { get; init; }
    }
}
