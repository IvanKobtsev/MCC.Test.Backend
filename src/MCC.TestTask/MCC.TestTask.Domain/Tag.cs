namespace MCC.TestTask.Domain;

public class Tag
{
    public Guid Id { get; set; }

    public DateTime CreateTime { get; set; }

    public string Name { get; set; }
    
    public Guid? AuthorId { get; set; }
    public User? Author { get; set; }
}