using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Visits
{
    [Key]
    public int Id { get; set; }
    
    public string VisiterId { get; set; }

    public string Path { get; set; }
    
    public Post? Post { get; set; }
}