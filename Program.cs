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
  Console.WriteLine("5) Delete Blog");
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
    Console.Write("Enter a name for a new Blog: ");
    var blog = new Blog { Name = Console.ReadLine() };

    ValidationContext context = new(blog, null, null);
    List<ValidationResult> results = [];

    var isValid = Validator.TryValidateObject(blog, context, results, true);
    if (isValid)
    {
      var db = new DataContext();
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
        // save blog to db
        db.AddBlog(blog);
        logger.Info("Blog added - {name}", blog.Name);
      }
    }
    if (!isValid)
    {
      foreach (var result in results)
      {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
      }
    }
  }
  else if (choice == "5")
  {
    // delete blog
    Console.WriteLine("Choose the blog to delete:");
  }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);

logger.Info("Program ended");
