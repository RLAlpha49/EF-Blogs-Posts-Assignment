using System.ComponentModel.DataAnnotations;

public class Post
{
  public int PostId { get; set; }
  
  [Required(ErrorMessage = "Title is required")]
  public string? Title { get; set; }
  
  [Required(ErrorMessage = "Content is required")]
  public string? Content { get; set; }

  public int BlogId { get; set; }
  public Blog? Blog { get; set; }
}
