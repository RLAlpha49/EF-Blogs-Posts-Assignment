using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{
  Console.WriteLine("Enter your selection:");
  Console.WriteLine("1) Display all blogs");
  Console.WriteLine("2) Add Blog");
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

    var db = new DataContext();
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", blog.Name);
  }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);

logger.Info("Program ended");
