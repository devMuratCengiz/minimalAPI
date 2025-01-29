using System.Text.Json;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string AtOccured => DateTime.Now.ToLongDateString();
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

