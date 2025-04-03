using NLog;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{
  Console.WriteLine("Enter your selection:");
  Console.WriteLine("1) Display all blogs");
  Console.WriteLine("2) Add Blog");
  Console.WriteLine("3) Create Post");
  Console.WriteLine("4) Display Posts");
  Console.WriteLine("Enter to quit");
  string? choice = Console.ReadLine();
  Console.Clear();
  logger.Info("Option {choice} selected", choice);

  if (choice == "1")
  {
    // display blogs
    var db = new DataContext();
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine($"{query.Count()} Blogs returned");
    foreach (var item in query)
    {
      Console.WriteLine(item.Name);
    }
  }
  else if (choice == "2")
  {
    // Add blog
    var db = new DataContext();
    Blog? blog = InputBlog(db, logger);
    if (blog != null)
    {
      //blog.BlogId = BlogId;
      db.AddBlog(blog);
      logger.Info("Blog added - {name}", blog.Name);
    }
  }
  else if (choice == "3")
  {
    // Create Post
    var db = new DataContext();
    // Prompt user to select a blog
    Console.WriteLine("Select the Blog you want to post to:");
    var blog = GetBlog(db);
    if (blog != null)
    {
      // Get post details
      Post? post = InputPost(blog, logger);
      if (post != null)
      {
        db.AddPost(post);
        logger.Info("Post added - {title} to blog {blog}", post.Title, blog.Name);
      }
    }
    else
    {
      Console.WriteLine("No blog selected or invalid blog ID.");
      logger.Error("Post creation failed - no blog selected");
    }
  }
  else if (choice == "4")
  {
    // Display Posts
    var db = new DataContext();
    // Prompt user to select a blog
    Console.WriteLine("Select the Blog whose posts you want to view:");
    var blog = GetBlog(db);
    if (blog != null)
    {
      // Display posts for the selected blog
      DisplayBlogPosts(db, blog.BlogId);
    }
    else
    {
      Console.WriteLine("No blog selected or invalid blog ID.");
      logger.Error("Post display failed - no blog selected");
    }
  }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);

logger.Info("Program ended");

static Blog? GetBlog(DataContext db)
{
  // display all blogs
  var blogs = db.Blogs.OrderBy(b => b.BlogId);
  foreach (Blog b in blogs)
  {
    Console.WriteLine($"{b.BlogId}: {b.Name}");
  }
  if (int.TryParse(Console.ReadLine(), out int BlogId))
  {
    Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId)!;
    return blog;
  }
  return null;
}

static Blog? InputBlog(DataContext db, NLog.Logger logger)
{
  Blog blog = new();
  Console.WriteLine("Enter the Blog name");
  blog.Name = Console.ReadLine();

  ValidationContext context = new(blog, null, null);
  List<ValidationResult> results = [];

  var isValid = Validator.TryValidateObject(blog, context, results, true);
  if (isValid)
  {
    // check for unique name
    if (db.Blogs.Any(b => b.Name == blog.Name))
    {
      // generate validation error
      isValid = false;
      results.Add(new ValidationResult("Blog name exists", ["Name"]));
    }
    else
    {
      logger.Info("Validation passed");
    }
  }
  if (!isValid)
  {
    foreach (var result in results)
    {
      logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }
    return null;
  }
  return blog;
}

static Post? InputPost(Blog blog, NLog.Logger logger)
{
  Post post = new();
  post.BlogId = blog.BlogId;
  post.Blog = blog;
  
  Console.WriteLine("Enter the Post title:");
  post.Title = Console.ReadLine();
  
  Console.WriteLine("Enter the Post content:");
  post.Content = Console.ReadLine();

  ValidationContext context = new(post, null, null);
  List<ValidationResult> results = [];

  var isValid = Validator.TryValidateObject(post, context, results, true);
  if (isValid)
  {
    logger.Info("Post validation passed");
  }
  else
  {
    foreach (var result in results)
    {
      logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }
    return null;
  }
  return post;
}

static void DisplayBlogPosts(DataContext db, int blogId)
{
  var posts = db.Posts.Where(p => p.BlogId == blogId).ToList();
  var blog = db.Blogs.FirstOrDefault(b => b.BlogId == blogId);
  
  if (blog == null)
  {
    Console.WriteLine("Blog not found.");
    return;
  }
  
  Console.WriteLine($"Posts for blog '{blog.Name}' (Total: {posts.Count}):");
  
  if (posts.Count == 0)
  {
    Console.WriteLine("No posts found for this blog.");
    return;
  }
  
  foreach (var post in posts)
  {
    Console.WriteLine($"Blog: {blog.Name}");
    Console.WriteLine($"Title: {post.Title}");
    Console.WriteLine($"Content: {post.Content}");
    Console.WriteLine(new string('-', 30));
  }
}
