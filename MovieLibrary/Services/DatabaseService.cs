using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLibrary.Services
{
    public class DatabaseService
    {
        MovieContext context = new MovieContext();

        //List of movies based on the number requested
        public List<Movie> GetMovies(int numberOfMovies)
        {
            var movies = context.Movies;
            var limitedMovies = movies.Take(numberOfMovies).ToList();
            return limitedMovies;
        }

        //List of movies with a specific name
        public List<Movie> GetMoviesByName(string movieName, string? searchType)
        {
            var movies = context.Movies;
            List<Movie> mov = movies.ToList();
            if (searchType == "Search")
            {
                //Case insensitive when it's for a "Search" request
                mov = movies.ToList().Where(m => m.Title.Contains(movieName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else if (searchType == "Add")
            {
                //Must be an exact match when it's for an "Add" request
                mov = movies.ToList().Where(m => m.Title == movieName).ToList();
            }
            return mov;
        }
        
        //Returns a movie object for a specific movie id
        public Movie GetMoviesById(long movieId)
        {
            var mov = context.Movies.FirstOrDefault(x => x.Id == movieId);
            return mov;
        }
        
        //Returns a movie list for a specific movie id
        public List<Movie> GetMovieListById(long movieId)
        {
            var movies = context.Movies;
            List<Movie> mov = movies.ToList();
            mov = movies.ToList().Where(x => x.Id == movieId).ToList();
            return mov;
        }
        
        //Adds the movie sent
        public void MovieAdd(Movie movie)
        {
            context.Movies.Add(movie);
            context.SaveChanges();
        }
        
        //Updates a movie with a new title
        public void MovieUpdate(Movie movie,string newTitle)
        {
            movie.Title = newTitle;
            context.SaveChanges();
        }

        //Deletes a specific movie
        public void MovieDelete(Movie movie)
        {
            context.Movies.Remove(movie);
            context.SaveChanges();
        }

        //Returns a list of genres
        public List<Genre> GetGenres()
        {
            var genres = context.Genres;
            return genres.ToList();
        }
        
        //Returns a genre that has a particular id
        public Genre GetGenresById(int genreId)
        {
            var gen = context.Genres.FirstOrDefault(x => x.Id == genreId);
            return gen;
        }

        //Add the MovieGenre sent
        public void MovieGenreAdd(MovieGenre movieGenre)
        {
            context.MovieGenres.Add(movieGenre);
            context.SaveChanges();
        }
    }
}
