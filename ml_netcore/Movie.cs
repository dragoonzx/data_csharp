using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;

namespace ml_assignment
{
    class Movie
    {
        public List<MovieNames> titles { get; set; }
        public HashSet<string> actorsSet { get; set; }
        public HashSet<string> producers { get; set; }
        public HashSet<string> tagsSet { get; set; }
        public string rating { get; set; }

        public List<Movie> similarMovies = new List<Movie>();

        public HashSet<string> TagsSet
        {
            get => tagsSet;
        }

        public Movie(List<MovieNames> titles, HashSet<string> actorsSet, HashSet<string> producers, HashSet<string> tagsSet, string rating)
        {
            this.titles = titles;
            this.actorsSet = actorsSet;
            this.producers = producers;
            this.tagsSet = tagsSet;
            this.rating = rating;
        }

        public void Similarity(Movie movie)
        {
            double similarityRating = 0.0;

            int tagsSimilarity = 0;
            int actorsSimilarity = 0;

            if (this.actorsSet != null && movie.actorsSet != null)
            {
                HashSet<string> intersectActors = new HashSet<string>(this.actorsSet);
                intersectActors.IntersectWith(movie.actorsSet);
                actorsSimilarity = intersectActors.Count;
            }

            if (this.tagsSet != null && movie.tagsSet != null)
            {
                HashSet<string> intersectTags = new HashSet<string>(this.tagsSet);
                intersectTags.IntersectWith(movie.tagsSet);
                tagsSimilarity = intersectTags.Count;
            }

            similarityRating += ((double)actorsSimilarity + tagsSimilarity) / 20;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            similarityRating += Convert.ToDouble(movie.rating, nfi) * 0.05;

            if (similarityRating > 0.8)
            {
                this.similarMovies.Add(movie);

            }

        }

        public string SimilarMoviesToString()
        {
            string movies = "";
            if (this.similarMovies != null)
            {
                foreach (Movie movie in this.similarMovies)
                {
                    movies += movie.ToString() + "\n";
                }
            }
            else
            {
                movies = "unknown";
            }

            return movies;
        }

        private string mapProducersToString()
        {
            string producers = "";
            if (this.producers != null)
            {
                foreach (string producer in this.producers)
                {
                    producers += producer + ", ";
                }
            }
            else
            {
                producers = "unknown";
            }

            return producers;
        }
        private string mapActorsToString()
        {
            string actors = "";
            if (this.actorsSet != null)
            {
                foreach (string actor in this.actorsSet)
                {
                    actors += actor + ", ";
                }
            }
            else
            {
                actors = "unknown";
            }

            return actors;
        }
        private string mapTagsToString()
        {
            string tags = "";
            if (this.tagsSet != null)
            {
                foreach (string tag in this.tagsSet)
                {
                    tags += tag + ", ";
                }
            }
            else
            {
                tags = "unknown";
            }

            return tags;
        }
        private string mapTitlesToString()
        {
            string titles = "";
            if (this.titles != null)
            {
                foreach (MovieNames title in this.titles)
                {
                    titles += title.FilmName + " language: " + title.FilmLang + ", ";
                }
            }
            else
            {
                titles = "unknown";
            }

            return titles;
        }
        public override string ToString()
        {
            return $"Titles are: {this.mapTitlesToString()}\nActors are: {this.mapActorsToString()}\nProducers are: {this.mapProducersToString()}\nTags are:{this.mapTagsToString()}\nRating is {this.rating}";
        }
    }
}
