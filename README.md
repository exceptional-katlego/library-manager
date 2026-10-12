# Library Manager (C#)

A console app for managing a small library: add, search, borrow, return and remove books. Books are saved to a JSON file, so they are still there the next time you open the app.

This project started as a first-year C# practical (2025), where the program stored two books in a fixed array. I rebuilt it to practise better design, error handling, saving data and unit testing.

## Features

- Add books with validation (title, author surname and page count are checked as you type)
- List all books in a table
- Search by title or author (not case sensitive)
- Borrow and return books, with checks (you cannot borrow a book that is already out)
- Remove books (with a confirmation, and not while a book is borrowed)
- Statistics: total, available, borrowed, average pages, longest book
- Saves to `books.json` automatically after every change
- Bad input never crashes the app: it shows a clear message and asks again
- 31 unit tests with xUnit

## Validation rules

| Field | Rule |
|---|---|
| Title | At least 3 characters |
| Author surname | Starts with a capital letter. Letters, spaces, hyphens and apostrophes only (so "Van der Merwe" and "O'Brien" work) |
| Pages | Whole number from 50 to 2000 |
| Duplicates | The same title by the same author cannot be added twice |

## Example

```
-------------- MENU --------------
1. Add a book
2. List all books
3. Search books
4. Borrow a book
5. Return a book
6. Remove a book
7. Statistics
0. Exit
----------------------------------
Choose an option: 1

Title: It
Title must have at least 3 characters. You entered: "It"
Title: The Hobbit
Author surname: tolkien
Author surname must start with a capital letter. You entered: "tolkien"
Author surname: Tolkien
Pages (50-2000): abc
"abc" is not a whole number. Please try again.
Pages (50-2000): 310
Added: [3] The Hobbit by Tolkien (310 pages) - Available

Choose an option: 2

ID   Title                          Author           Pages  Status
---------------------------------------------------------------------------
1    Things Fall Apart              Achebe             209  Borrowed (Thabo)
2    Long Walk to Freedom           Mandela            656  Available
3    The Hobbit                     Tolkien            310  Available
```

## How to run

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download) (or newer).

**Visual Studio:** open `LibraryManager.sln`, set `LibraryManager` as the startup project and press F5.

**Command line:**

```
cd LibraryManager
dotnet run
```

**Run the tests:**

```
dotnet test
```

## Project structure

```
LibraryManager/
├── Program.cs              Menu and user interaction
├── Models/Book.cs          The Book class and its validation rules
├── Services/Library.cs     Add, search, borrow, return, remove, statistics
├── Services/BookStorage.cs Save and load books as JSON
├── Helpers/InputHelper.cs  Safe input (no crashes on bad input)
└── Exceptions/BookException.cs  Custom exception for invalid book details

LibraryManager.Tests/       xUnit tests for Book, Library and BookStorage
```

## What I practised

- **Classes and encapsulation:** Book properties have private setters, so a book can only change through methods like `Borrow()` and `Return()`.
- **Validation in one place:** the rules live in the Book class. The menu and the constructor both use them, so they can never disagree.
- **Custom exceptions:** `BookException` says which field was wrong (`FieldName`) and why.
- **Collections and LINQ:** `List<Book>` instead of a fixed array, with `Where`, `Any`, `Max`, `Average` and `OrderBy`.
- **File handling:** saving and loading JSON with `System.Text.Json`. If the file is damaged, the app makes a backup and starts fresh instead of crashing.
- **Safe input:** `int.TryParse` instead of `int.Parse`, so typing letters for a number does not crash the app.
- **Separation of concerns:** the menu code, the library logic and the file saving are in separate classes, which makes the logic easy to test.
- **Unit testing:** xUnit `[Fact]` and `[Theory]` tests, including edge cases like the 50 and 2000 page limits.

## Possible next steps

- Track due dates and show overdue books
- Add members with a borrowing limit
- Swap the JSON file for a SQLite database
- Build a simple web or desktop interface on top of the same `Library` class

## License

MIT. See [LICENSE](LICENSE).
