using System.ComponentModel.DataAnnotations;
using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string? choice;

do
{
    // Display the choices to the user
    logger.Info("Entered Menu Options");
    Console.WriteLine("1) Display All Blogs");
    Console.WriteLine("2) Add Blog");
    Console.WriteLine("3) Create Post");
    Console.WriteLine("4) Display Posts");
    Console.WriteLine("5) Delete Blog");
    Console.WriteLine("6) Edit Blog");
    Console.WriteLine("Enter to quit");


    // Input selection 
    choice = Console.ReadLine();
    logger.Info("User choice: {Choice}", choice);

    if (choice == "1")
    {
        // Display all blogs from the database
        var db = new DataContext();
        var query = db.Blogs.OrderBy(b => b.Name);

        Console.WriteLine("All blogs in the database:");

        foreach (var item in query)
        {
            Console.WriteLine(item.Name);
        }

    }
    else if (choice == "2")
    {
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
        // Create a Post for a Blog
        // Display all blogs from the database
        var db = new DataContext();
        var query = db.Blogs.OrderBy(b => b.Name);

        Console.WriteLine("All blogs in the database:");
        foreach (var item in query)
        {
            Console.WriteLine(item.Name);
        }

        Console.Write("Enter the name of the Blog to add a Post to: ");
        var blogName = Console.ReadLine();

        var blog = db.Blogs.FirstOrDefault(b => b.Name == blogName);

        if (blog != null)
        {
            Console.Write("Enter the title of the Post: ");
            var title = Console.ReadLine();
            Console.Write("Enter the content of the Post: ");
            var content = Console.ReadLine();

            var post = new Post { Title = title, Content = content, Blog = blog };
            db.AddPost(post);
            logger.Info("Post added - {title}", title);
        }
        else
        {
            Console.WriteLine("Blog not found");
            logger.Info("Blog not found - {blogName}", blogName);
        }

    }
    else if (choice == "4")
    {
        // Display all posts from a selected blog
        var db = new DataContext();
        var blogs = db.Blogs.ToList();

        if (blogs.Count == 0)
        {
            Console.WriteLine("No blogs available.");
            logger.Info("No blogs available.");
        }
        else
        {
            Console.WriteLine("Select a blog to view its posts:");
            for (int i = 0; i < blogs.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {blogs[i].Name}");
            }

            int blogChoice;
            while (!int.TryParse(Console.ReadLine(), out blogChoice) || blogChoice < 1 || blogChoice > blogs.Count)
            {
                Console.WriteLine("Invalid choice. Please select a valid blog number.");
            }

            var selectedBlog = blogs[blogChoice - 1];
            var posts = db.Posts.Where(p => p.BlogId == selectedBlog.BlogId).ToList();

            Console.WriteLine($"All posts in the blog '{selectedBlog.Name}':");
            if (posts.Count == 0)
            {
                Console.WriteLine("No posts available.");
            }
            else
            {
                foreach (var post in posts)
                {
                    Console.WriteLine($"Blog: {selectedBlog.Name}");
                    Console.WriteLine($"Title: {post.Title}");
                    Console.WriteLine($"Content: {post.Content}");
                    Console.WriteLine();
                }
                Console.WriteLine($"Total number of posts: {posts.Count}");
            }
        }
    }
    else if (choice == "5")
    {
        // delete blog
        Console.WriteLine("Choose the blog to delete:");
        var db = new DataContext();
        var blog = GetBlog(db);
        if (blog != null)
        {
            // TODO: delete blog
            logger.Info($"Blog (id: {blog.BlogId}) deleted");
        }
        else
        {
            logger.Error("Blog is null");
        }
    }
    else if (choice == "6")
    {
        // edit blog
        Console.WriteLine("Choose the blog to edit:");
        var db = new DataContext();
        var blog = GetBlog(db);
        if (blog != null)
        {
            // input blog
            Blog? UpdatedBlog = InputBlog(db, logger);
            if (UpdatedBlog != null)
            {
                UpdatedBlog.BlogId = blog.BlogId;
                db.EditBlog(UpdatedBlog);
                logger.Info($"Blog (id: {blog.BlogId}) updated");
            }
        }
    }
} while (choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6");


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