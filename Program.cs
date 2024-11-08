﻿using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string? choice;

do {
  // Display the choices to the user
  logger.Info("Entered Menu Options");
  Console.WriteLine("1) Display All Blogs");
  Console.WriteLine("2) Add Blog");
  Console.WriteLine("3) Create Post");
  Console.WriteLine("4) Display Posts");
  Console.WriteLine("Enter to quit");


// Input selection 
choice = Console.ReadLine();
logger.Info("User choice: {Choice}", choice);

} while (choice == "1" || choice == "2" || choice == "3" || choice == "4");
// Create and save a new Blog
Console.Write("Enter a name for a new Blog: ");
var name = Console.ReadLine();

var blog = new Blog { Name = name };

var db = new DataContext();
db.AddBlog(blog);
logger.Info("Blog added - {name}", name);

// Display all Blogs from the database
var query = db.Blogs.OrderBy(b => b.Name);

Console.WriteLine("All blogs in the database:");
foreach (var item in query)
{
  Console.WriteLine(item.Name);
}

logger.Info("Program ended");