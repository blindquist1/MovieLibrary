//using Castle.DynamicProxy.Generators;
using Microsoft.EntityFrameworkCore;
using MovieLibrary.Services;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int menuSelection = 0;
            int maxMenuItem = 6;
            while (menuSelection != maxMenuItem)
            {
                Console.WriteLine("Menu Options");
                Console.WriteLine("1. List Movies");
                Console.WriteLine("2. Update Movie");
                Console.WriteLine("3. Add Movie");
                Console.WriteLine("4. Search Movies");
                Console.WriteLine("5. Delete Movie");
                Console.WriteLine("6. Exit");
                Console.WriteLine();

                bool validEntry = false;

                //Keep looping through until user chooses a valid entry, an integer and between 1 and 4.
                while (!validEntry)
                {
                    menuSelection = InputService.GetIntWithPrompt("Select an option: ", "Entry must be an integer");
                    if (menuSelection < 1 || menuSelection > maxMenuItem)
                    {
                        Console.WriteLine($"Entry must be between 1 and {maxMenuItem}");
                    }
                    else
                    {
                        validEntry = true;
                    }
                }

                Console.WriteLine();

                // READ Movies
                if (menuSelection == 1)
                {
                    int movieCount = 0;
                    while (movieCount < 1)
                    {
                        movieCount = InputService.GetIntWithPrompt("How many movies do you want to display? ", "Entry must be an integer");
                    }

                    DatabaseService queries = new DatabaseService();

                    var movieList = queries.GetMovies(movieCount);

                    ListMovies(movieList);
                }

                // UPDATE Movies
                if (menuSelection == 2)
                {
                    SelectMovie("update");
                }

                // Add Movie
                if (menuSelection == 3)
                {
                    var movieTitle = InputService.GetStringWithPrompt("Enter a movie title: ", "Entry must be a string");
                    DatabaseService queries = new DatabaseService();

                    var movieList = queries.GetMoviesByName(movieTitle,"Add");

                    if (movieList.Count > 0)
                    {
                        Console.WriteLine("That movie already exists");
                    }
                    else
                    {
                        DateTime releaseDate = InputService.GetDateWithPrompt("Enter a release date: ", "Entry must be a date");
                        var mov = new Movie();
                        mov.Title = movieTitle;
                        mov.ReleaseDate = releaseDate;

                        queries.MovieAdd(mov);

                        //Prompt the user to select from the available genres
                        int genreChoice = 9999;
                        while(genreChoice != 0)
                        {
                            var genreList = queries.GetGenres();
                            ListGenres(genreList);
                       
                            genreChoice = InputService.GetIntWithPrompt("Pick a genre to add to the movie, enter 0 when done: ", "Entry must be an integer");
                        
                            if (genreChoice != 0)
                            {
                                var gen = new Genre();
                                gen = queries.GetGenresById(genreChoice);
                                if (gen != null)
                                {
                                    var movGen = new MovieGenre();
                                    movGen.Movie = mov;
                                    movGen.Genre = gen;

                                    queries.MovieGenreAdd(movGen);
                                }
                                else if (genreChoice != 0)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("That is an invalid selection, please try again");
                                }
                            }
                        }
                        var movieAddedList = queries.GetMovieListById(mov.Id);
                        ListMovies(movieAddedList);
                    }
                }
                // SEARCH Movies
                if (menuSelection == 4)
                {
                    var movieSelection = InputService.GetStringWithPrompt("Enter a movie to search for: ", "Entry must be a string");
                    DatabaseService queries = new DatabaseService();

                    var movieList = queries.GetMoviesByName(movieSelection,"Search");

                    ListMovies(movieList);
                }

                // DELETE Movies
                if (menuSelection == 5)
                {
                    SelectMovie("delete");
                }
                Console.WriteLine();
            }

        }
        
        //Display list of movies and their corresponding genres
        static void ListMovies(List<Movie> movieList)
        {
            Console.WriteLine();

            if (movieList.Any())
            {
                if(movieList.Count == 1)
                {
                    Console.WriteLine("The movie is:");
                }
                else
                {
                    Console.WriteLine("The movies are:");
                }
                Console.WriteLine();

                foreach (var movie in movieList)
                {
                    Console.WriteLine($"{movie.Id} {movie.Title} {movie.ReleaseDate:d}");

                    foreach (var movieGenre in movie.MovieGenres)
                    {
                        Console.WriteLine($"  {movieGenre.Genre.Name}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No movies found");
            }

        }
        
        //Display list of genres
        static void ListGenres(List<Genre> genreList)
        {
            Console.WriteLine();

            if (genreList.Any())
            {
                Console.WriteLine("The genres are:");

                foreach (var genre in genreList)
                {
                    Console.WriteLine($"{genre.Id} {genre.Name}");
                }
            }
            else
            {
                Console.WriteLine("No genres found");
            }

        }
        
        //Prompt user for a movie to either update or delete
        static void SelectMovie(string selectType)
        {
            var movieSelection = InputService.GetStringWithPrompt($"Enter the title of a movie to {selectType}: ", "Entry must be a string");
            DatabaseService queries = new DatabaseService();
            var movieList = queries.GetMoviesByName(movieSelection, "Search");

            var mov = new Movie();

            if (movieList.Count == 1)
            {
                mov = queries.GetMoviesById(movieList[0].Id);
            }
            //If more than one movie found, prompt user to select from the reduced list
            else if (movieList.Count > 1)
            {
                ListMovies(movieList);
                Console.WriteLine();
                do
                {
                    long movieChoice = InputService.GetIntWithPrompt($"Several movies found, please enter the id number of the one you want to {selectType}: ", "Entry must be an integer");
                    mov = queries.GetMoviesById(movieChoice);
                    if (mov == null)
                    {
                        Console.WriteLine("That id is invalid");
                    }
                } while (mov == null);
            }
            
            //If a movie is found either delete or update it based on the select type
            if (movieList.Count > 0)
            {
                if (selectType == "delete")
                {
                    var title = mov.Title;
                    queries.MovieDelete(mov);
                    Console.WriteLine();
                    Console.WriteLine($"Movie {title} is now deleted.");
                }
                else if (selectType == "update")
                {
                    var movieTitle = InputService.GetStringWithPrompt("Enter the updated movie title: ", "Entry must be a string");
                    queries.MovieUpdate(mov, movieTitle);
                    var movieUpdatedList = queries.GetMovieListById(mov.Id);
                    ListMovies(movieUpdatedList);
                }
            }
            //If no movie found, tell the end user
            else
            {
                Console.WriteLine("That movie was not found.");
            }
        }
    }
}