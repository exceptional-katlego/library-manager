using LibraryManager.Exceptions;
using LibraryManager.Helpers;
using LibraryManager.Models;
using LibraryManager.Services;

namespace LibraryManager
{
    internal class Program
    {
        private const string DataFile = "books.json";

        private static Library _library = null!;
        private static BookStorage _storage = null!;

        static void Main(string[] args)
        {
            _storage = new BookStorage(DataFile);
            _library = new Library(_storage.Load());

            Console.WriteLine("=====================================");
            Console.WriteLine("        LIBRARY MANAGER (C#)");
            Console.WriteLine("=====================================");
            Console.WriteLine($"Loaded {_library.AllBooks.Count} book(s) from {DataFile}.");

            bool running = true;
            while (running)
            {
                ShowMenu();
                int choice = InputHelper.ReadInt("Choose an option: ");
                Console.WriteLine();

                // One try/catch around every action: bad input shows a message
                // and returns to the menu instead of crashing the program.
                try
                {
                    switch (choice)
                    {
                        case 1: AddBook(); break;
                        case 2: ListBooks(); break;
                        case 3: SearchBooks(); break;
                        case 4: BorrowBook(); break;
                        case 5: ReturnBook(); break;
                        case 6: RemoveBook(); break;
                        case 7: ShowStats(); break;
                        case 0: running = false; break;
                        default: InputHelper.WriteError("Please choose a number from the menu."); break;
                    }
                }
                catch (BookException ex)
                {
                    InputHelper.WriteError($"Invalid {ex.FieldName}: {ex.Message}");
                }
                catch (Exception ex) when (ex is InvalidOperationException
                                           || ex is KeyNotFoundException
                                           || ex is ArgumentException)
                {
                    InputHelper.WriteError(ex.Message);
                }
                catch (IOException ex)
                {
                    InputHelper.WriteError($"Could not save to {DataFile}: {ex.Message}");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Goodbye!");
        }

        private static void ShowMenu()
        {
            Console.WriteLine("-------------- MENU --------------");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. List all books");
            Console.WriteLine("3. Search books");
            Console.WriteLine("4. Borrow a book");
            Console.WriteLine("5. Return a book");
            Console.WriteLine("6. Remove a book");
            Console.WriteLine("7. Statistics");
            Console.WriteLine("0. Exit");
            Console.WriteLine("----------------------------------");
        }

        private static void AddBook()
        {
            // Each field is checked as soon as it is typed, using the same rules as the Book class.
            string title = AskUntilValid(() => Book.ValidateTitle(InputHelper.ReadText("Title: ")));
            string surname = AskUntilValid(() => Book.ValidateSurname(InputHelper.ReadText("Author surname: ")));
            int pages = AskUntilValid(() =>
                Book.ValidatePages(InputHelper.ReadInt($"Pages ({Book.MinPages}-{Book.MaxPages}): ")));

            Book book = _library.AddBook(title, surname, pages);
            Save();
            InputHelper.WriteSuccess($"Added: {book}");
        }

        private static void ListBooks()
        {
            if (_library.AllBooks.Count == 0)
            {
                Console.WriteLine("The library is empty. Choose option 1 to add a book.");
                return;
            }

            PrintTable(_library.AllBooks.OrderBy(b => b.Id));
        }

        private static void SearchBooks()
        {
            string term = InputHelper.ReadRequiredText("Search title or author: ");
            List<Book> results = _library.Search(term);

            if (results.Count == 0)
            {
                Console.WriteLine($"No books match \"{term}\".");
                return;
            }

            Console.WriteLine($"{results.Count} result(s):");
            PrintTable(results);
        }

        private static void BorrowBook()
        {
            int id = InputHelper.ReadInt("Book ID to borrow: ");
            string name = InputHelper.ReadRequiredText("Borrower's name: ");

            _library.BorrowBook(id, name);
            Save();
            InputHelper.WriteSuccess($"Book {id} borrowed by {name}.");
        }

        private static void ReturnBook()
        {
            int id = InputHelper.ReadInt("Book ID to return: ");

            _library.ReturnBook(id);
            Save();
            InputHelper.WriteSuccess($"Book {id} returned.");
        }

        private static void RemoveBook()
        {
            int id = InputHelper.ReadInt("Book ID to remove: ");
            Book book = _library.FindById(id)
                ?? throw new KeyNotFoundException($"No book found with ID {id}.");

            if (!InputHelper.Confirm($"Remove \"{book.Title}\"?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            _library.RemoveBook(id);
            Save();
            InputHelper.WriteSuccess($"Removed \"{book.Title}\".");
        }

        private static void ShowStats()
        {
            LibraryStats stats = _library.GetStats();

            Console.WriteLine($"Total books:    {stats.Total}");
            Console.WriteLine($"Available:      {stats.Available}");
            Console.WriteLine($"Borrowed:       {stats.Borrowed}");
            Console.WriteLine($"Average pages:  {stats.AveragePages:F1}");
            if (stats.Longest != null)
            {
                Console.WriteLine($"Longest book:   {stats.Longest.Title} ({stats.Longest.Pages} pages)");
            }
        }

        private static void PrintTable(IEnumerable<Book> books)
        {
            Console.WriteLine($"{"ID",-4} {"Title",-30} {"Author",-15} {"Pages",6}  Status");
            Console.WriteLine(new string('-', 75));
            foreach (Book b in books)
            {
                string title = b.Title.Length > 30 ? b.Title[..27] + "..." : b.Title;
                string status = b.IsBorrowed ? $"Borrowed ({b.BorrowedBy})" : "Available";
                Console.WriteLine($"{b.Id,-4} {title,-30} {b.AuthorSurname,-15} {b.Pages,6}  {status}");
            }
        }

        /// <summary>
        /// Keeps running the "ask" step until it does not throw a BookException.
        /// Generic (T) so it works for text and numbers.
        /// </summary>
        private static T AskUntilValid<T>(Func<T> ask)
        {
            while (true)
            {
                try
                {
                    return ask();
                }
                catch (BookException ex)
                {
                    InputHelper.WriteError(ex.Message);
                }
            }
        }

        private static void Save()
        {
            _storage.Save(_library.AllBooks);
        }
    }
}
