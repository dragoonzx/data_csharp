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
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace ml_assignment
{
    static class Program
    {
        static void Main(string[] args)
        {
            //mongodb = connection to moviebase db and connecting to three separate collections: movies, actors and tags
            //var client = new MongoClient("mongodb://localhost:27017");
            //var database = client.GetDatabase("moviebase");
            //var moviesCollection = database.GetCollection<BsonDocument>("movies");
            //var actorsCollection = database.GetCollection<BsonDocument>("actors");
            //var tagsCollection = database.GetCollection<BsonDocument>("tags");
            //var testCollection = database.GetCollection<BsonDocument>("test");

            //var document = new BsonDocument
            //{
            //    { "name", "MongoDB" },
            //    { "type", "Database" },
            //    { "count", 1 },
            //    { "info", new BsonDocument
            //        {
            //            { "x", 203 },
            //            { "y", 102 }
            //        }}
            //};
            //collection.InsertOne(document);

            string path1 = "../../../ml-latest/movieCodes_IMDB.tsv";
            string path2 = "../../../ml-latest/ActorsDirectorsNames_IMDB.txt";
            string path3 = "../../../ml-latest/Ratings_IMDB.tsv";
            string path4 = "../../../ml-latest/ActorsDirectorsCodes_IMDB.tsv";
            string path5 = "../../../ml-latest/links_IMDB_MovieLens.csv";
            string path6 = "../../../ml-latest/TagCodes_MovieLens.csv";
            string path7 = "../../../ml-latest/TagScores_MovieLens.csv";

            var startTime = Stopwatch.StartNew();
            //Optimized
            //Console.WriteLine("Find Mov Work Start");
            //Dictionary<string, string> movs = findMov(path1);
            //Console.WriteLine("Find Mov Work Done");

            Console.WriteLine("Movie Dictionary?");
            Dictionary<string, List<MovieNames>> movieDictionary = createDictionary1(path1);
            Console.WriteLine("Movie Dic end");
            Console.WriteLine("Staff and rating");
            Dictionary<string, string> staffDictionary = createDictionary2(path2);
            Dictionary<string, string> ratingDictionary = findRating(path3);
            Console.WriteLine("Staff and rating end");
            //OPTIMIZE IT!!!
            Dictionary<string, HashSet<string>> actorsDictionary = new Dictionary<string,HashSet<string>>();
            Dictionary<string, HashSet<string>> producersDictionary = new Dictionary<string, HashSet<string>>();
            //Action<object> action1 = (object obj) =>
            //{
            //    Console.WriteLine("Task={0}, obj={1}, Thread={2}",
            //    Task.CurrentId, obj,
            //    Thread.CurrentThread.ManagedThreadId);
            //    //creating actors dictionary
            //    actorsDictionary = findActors(path4, staffDictionary);
            //};
            //Task t1 = new Task(action1, "findActors");
            //Action<object> action2 = (object obj) =>
            //{
            //    Console.WriteLine("Task={0}, obj={1}, Thread={2}",
            //    Task.CurrentId, obj,
            //    Thread.CurrentThread.ManagedThreadId);
            //    //creating producers dictionary
            //    producersDictionary = findProducers(path4, staffDictionary);
            //};
            //Task t2 = new Task(action2, "findProducers");
            Parallel.Invoke(() => actorsDictionary = findActors(path4, staffDictionary),
                () => producersDictionary = findProducers(path4,staffDictionary));
            //Task[] tasks = new Task[2];
            //tasks.Append(t1);
            //tasks.Append(t2);
            //Task.WaitAll(tasks);
            //t1.Start();
            //t2.Start();


            //List<Dictionary<string, HashSet<string>>> actorsProducersList = findActorsAndProducers(path4, staffDictionary);
            Dictionary<string, string> linksDictionary = findLinks(path5);
            Dictionary<string, string> tagsDictionary = getTagDict(path6);
            Dictionary<string, HashSet<string>> tagsMovieDictionary = findTags(path7, linksDictionary, tagsDictionary);
            
            //t2.Wait();
            //t1.Wait();

            //key(code)-Movie
            Dictionary<string, Movie> movies = getMovies(path3, movieDictionary, actorsDictionary, producersDictionary, tagsMovieDictionary, ratingDictionary);

            Dictionary<string, HashSet<Movie>> staff = getStaffFilms(path2, movies);

            //Dictionary<string, HashSet<Movie>> tags = getTagsFilms(path6, movies);

            Console.WriteLine("hey guys");
            void kek(Movie film)
            {
                foreach (KeyValuePair<string, Movie> poop in movies)
                {
                    if (film.similarMovies.Count <= 10)
                    {
                        if (film.TagsSet != poop.Value.TagsSet)
                        {
                            film.Similarity(poop.Value);
                        }
                    }
                }
            };
            //ASSIGNMENT3 TEST
            //HashSet<Movie> val;
            //tags.TryGetValue("action", out val);
            //foreach(Movie fs in val)
            //{
            //    Console.WriteLine(fs.ToString());
            //}

            //ASSIGNMENT2 TEST
            //HashSet<Movie> val;
            //staff.TryGetValue("Guy Ritchie", out val);

            //foreach(Movie f in val)
            //{
            //    Console.WriteLine(f.ToString());
            //}

            //ASSIGNMENT1 TEST
            //foreach(KeyValuePair<string, Movie> movie in movies)
            //{
            //    Console.WriteLine("Key is: " + movie.Key + ", Values is: " + movie.Value.ToString());
            //}
            //Actors test
            //foreach (KeyValuePair<string, HashSet<string>> actor in actorsDictionary)
            //{
            //    for(var i = 0; i<actor.Value.Count;i++)
            //        Console.WriteLine("Key is: " + actor.Key + ", Values is: " + actor.Value);
            //}

            startTime.Stop();
            var resultTime = startTime.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                resultTime.Hours,
                resultTime.Minutes,
                resultTime.Seconds,
                resultTime.Milliseconds);
            Console.WriteLine(elapsedTime);
            Console.WriteLine("Im done");

            //Dictionary<string, Movie> UserX
            string exit = "no exit";
            do
            {
                Console.WriteLine("1. Распечатать информацию о фильме");
                Console.WriteLine("2. Распечатать информацию об актере-режиссере");
                Console.WriteLine("3. Распечатать информацию о тэге");
                Console.WriteLine("0. Выйти");
                string caseSwitch = Console.ReadLine();
                switch (caseSwitch)
                {
                    //case "1":
                    //    Console.WriteLine("Write right film name:");
                    //    string filmName = Console.ReadLine();

                    //    //string key = extractCode(path1, filmName.ToLower());
                    //    string key;
                    //    movs.TryGetValue(filmName, out key);
                    //    if (key != null)
                    //    {
                    //        Movie film;
                    //        movies.TryGetValue(key, out film);

                    //        Console.WriteLine(film.ToString());
                    //        kek(film);
                    //        Console.WriteLine("Similar Movies: \n" + film.SimilarMoviesToString());
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Sorry, we cant find film with that name");
                    //    }
                    //    Console.ReadLine();
                    //    break;
                    //case "2":
                    //    Console.WriteLine("Write right actor-producer name:");
                    //    string actorName = Console.ReadLine();
                    //    HashSet<Movie> val;
                    //    staff.TryGetValue(actorName, out val);
                    //    if (val != null)
                    //    {
                    //        Console.WriteLine(actorName + " MOVIES:");
                    //        foreach (Movie f in val)
                    //        {
                    //            Console.WriteLine(f.ToString());
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Sorry, we cant find actor-producer with that name");
                    //    }
                    //    Console.ReadLine();
                    //    break;
                    //case "3":
                    //    Console.WriteLine("Write right tag name:");
                    //    string tagName = Console.ReadLine();
                    //    HashSet<Movie> tag;
                    //    tags.TryGetValue(tagName.ToLower(), out tag);
                    //    if (tag != null)
                    //    {
                    //        Console.WriteLine(tagName + " MOVIES:");
                    //        foreach (Movie fs in tag)
                    //        {
                    //            Console.WriteLine(fs.ToString());
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("Sorry, we cant find that tag");
                    //    }
                    //    Console.ReadLine();
                    //    break;
                    case "0":
                        exit = "exit";
                        break;
                    default:
                        Console.WriteLine("Вы ввели неправильную команду");
                        break;
                }
            } while (exit != "exit");
            Console.ReadLine();
        }

        //private static Dictionary<string, string> findMov(string path)
        //{
        //    string[] line;
        //    Dictionary<string, string> movsDic = new Dictionary<string, string>();
        //    string[] movsString = File.ReadAllLines(path);


        //    for(int i = 1; i< movsString.Length; i++)
        //    {
        //        line = movsString[i].Split('\t');
        //        if(line[3].ToLower() == "ru" || line[3].ToLower() == "us" || line[3].ToLower() == "suhh")
        //        {
        //            if (!movsDic.ContainsKey(line[2]))
        //            {
        //                movsDic.Add(line[2], line[0]);
        //            }
        //        }
        //    }

        //    return movsDic;
        //}

        private static Dictionary<string, string> findMov(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, string> movsDic = new Dictionary<string, string>();
            string[] movsString;

            while ((line = file.ReadLine()) != null)
            {
                movsString = line.Split('\t');
                if (movsString[3].ToLower() == "ru" || movsString[3].ToLower() == "us" || movsString[3].ToLower() == "suhh")
                {
                    if (!movsDic.ContainsKey(movsString[2]))
                    {
                        movsDic.Add(movsString[2], movsString[0]);
                    }
                }
            }
            return movsDic;
        }

        private static Dictionary<string, HashSet<Movie>> getTagsFilms(string path, Dictionary<string, Movie> movies)
        {
            StreamReader file = new StreamReader(path);
            string line;
            string[] tagsString;
            HashSet<Movie> tagMovs = new HashSet<Movie>();
            Dictionary<string, HashSet<Movie>> tags = new Dictionary<string, HashSet<Movie>>();
            
            Console.WriteLine("Get tags!");
            while ((line = file.ReadLine()) != null)
            {
                tagsString = line.Split(',');
                if (tagsString[0] != "tagId")
                {
                    foreach (KeyValuePair<string, Movie> movie in movies)
                    {
                        if (movie.Value.TagsSet != null)
                        {
                            if (movie.Value.TagsSet.Contains(tagsString[1]))
                            {
                                tagMovs.Add(movie.Value);
                            }
                        }
                    }
                    tags.Add(tagsString[1], tagMovs);
                    
                    tagMovs = new HashSet<Movie>();
                }
            }
            return tags;
        }

        private static Dictionary<string, HashSet<Movie>> getStaffFilms(string path, Dictionary<string, Movie> movies)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, HashSet<Movie>> staffFilmsDic = new Dictionary<string, HashSet<Movie>>();
            HashSet<Movie> moviesSet = new HashSet<Movie>();
            string[] staffString;
            string[] movieCodes;
            Movie movie;
            
            Console.WriteLine("Get staff");
            while ((line = file.ReadLine()) != null)
            {
                staffString = line.Split('\t');
                if (staffString[0] != "nconst")
                {
                    movieCodes = staffString[5].Split(',');
                    foreach (string movieCode in movieCodes)
                    {
                        movies.TryGetValue(movieCode, out movie);
                        moviesSet.Add(movie);
                    }
                    if (!staffFilmsDic.ContainsKey(staffString[1]))
                    {
                        staffFilmsDic.Add(staffString[1], moviesSet);                        

                        moviesSet = new HashSet<Movie>();
                    }
                    else
                    {
                        moviesSet = new HashSet<Movie>();
                    }

                }

            }
            return staffFilmsDic;
        }

        private static Dictionary<string, Movie> getMovies(string path, Dictionary<string, List<MovieNames>> movieDictionary, Dictionary<string, HashSet<string>> actorsDictionary, Dictionary<string, HashSet<string>> producersDictionary, Dictionary<string, HashSet<string>> tagsDictionary, Dictionary<string, string> ratingDictionary)
        {
            StreamReader file = new StreamReader(path);
            string line;
            string[] codesString;
            List<MovieNames> titles;
            HashSet<string> actors;
            HashSet<string> producers;
            HashSet<string> tags;
            string rating;
            Dictionary<string, Movie> movieDic = new Dictionary<string, Movie>();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("moviebase");
            var testCollection = database.GetCollection<BsonDocument>("movies");

            Console.WriteLine("Get movies!");
            while ((line = file.ReadLine()) != null)
            {
                codesString = line.Split('\t');
                if (codesString[1] != "averageRating")
                {
                    movieDictionary.TryGetValue(codesString[0], out titles);
                    actorsDictionary.TryGetValue(codesString[0], out actors);
                    producersDictionary.TryGetValue(codesString[0], out producers);
                    tagsDictionary.TryGetValue(codesString[0], out tags);
                    ratingDictionary.TryGetValue(codesString[0], out rating);
                    Movie movie = new Movie(titles, actors, producers, tags, rating);
                    movieDic.Add(codesString[0], movie);
                    
                        var doc = new JObject
                        {
                            {"movieCode", codesString[0] },
                            {"movie", movie.ToJson() },
                            {"rating", rating }
                        };
                        var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
                        var bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonDoc);
                        testCollection.InsertOne(bsonDoc);
             

                }
            }

            return movieDic;
        }
        

        private static Dictionary<string, string> getTagDict(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, string> tagDic = new Dictionary<string, string>();
            string[] tagString;
            while ((line = file.ReadLine()) != null)
            {
                tagString = line.Split(',');
                tagDic.Add(tagString[0], tagString[1]);
            }
            return tagDic;
        }

        //private static Dictionary<string, HashSet<string>> findTags(string path,Dictionary<string,string> links, Dictionary<string,string> tags)
        //{
        //    string[] line = File.ReadAllLines(path);
        //    Dictionary<string, HashSet<string>> scoresDic = new Dictionary<string,HashSet<string>>();
        //    string[] scoresString;
        //    string initialCode = "tt0114709";
        //    HashSet<string> tagsSet = new HashSet<string>();
        //    double value=0;
        //    string tag;
        //    string code;

        //    NumberFormatInfo nfi = new NumberFormatInfo();
        //    nfi.NumberDecimalSeparator = ".";
        //    string codeZ;
        //    for(int i = 1; i< line.Length; i++)
        //    {
        //        scoresString = line[i].Split(',');
        //        links.TryGetValue(scoresString[0], out codeZ);
        //        if (codeZ != initialCode)
        //        {
        //            scoresDic.Add(initialCode, tagsSet);
        //            links.TryGetValue(scoresString[0], out code);
        //            initialCode = code;
        //            tagsSet = new HashSet<string>();
        //        }
        //        if(scoresString[2] !="relevance")
        //            value = Convert.ToDouble(scoresString[2], nfi);
        //        if (value >= 0.5)
        //        {

        //            links.TryGetValue(scoresString[0], out code);
        //            initialCode = code;

        //            tags.TryGetValue(scoresString[1], out tag);

        //            tagsSet.Add(tag);
        //        }
        //    }
        //    return scoresDic;
        //}

        private static Dictionary<string, HashSet<string>> findTags(string path, Dictionary<string, string> links, Dictionary<string, string> tags)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, HashSet<string>> scoresDic = new Dictionary<string, HashSet<string>>();
            string[] scoresString;
            string initialCode = "tt0114709";
            HashSet<string> tagsSet = new HashSet<string>();
            double value = 0;
            string tag;
            string code;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            string codeZ;
            while ((line = file.ReadLine()) != null)
            {
                scoresString = line.Split(',');
                links.TryGetValue(scoresString[0], out codeZ);
                if (codeZ != initialCode && scoresString[0] != "movieId")
                {
                    scoresDic.Add(initialCode, tagsSet);
                    links.TryGetValue(scoresString[0], out code);
                    initialCode = code;
                    tagsSet = new HashSet<string>();
                }
                if (scoresString[2] != "relevance")
                    value = Convert.ToDouble(scoresString[2], nfi);
                if (value >= 0.5)
                {

                    links.TryGetValue(scoresString[0], out code);
                    initialCode = code;

                    tags.TryGetValue(scoresString[1], out tag);

                    tagsSet.Add(tag);
                }
            }
            return scoresDic;
        }

        //KEY: tCode Value: movielens MovieID
        private static Dictionary<string, string> findLinks(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, string> linksDic = new Dictionary<string, string>();
            string[] linksString;
            while ((line = file.ReadLine()) != null)
            {
                linksString = line.Split(',');
                linksDic.Add(linksString[0], "tt" + linksString[1]);
            }
            return linksDic;
        }


        //private static List<Dictionary<string, HashSet<string>>> findActorsAndProducers(string path, Dictionary<string,string> staffDictionary)
        //{
        //    StreamReader file = new StreamReader(path);
        //    string line;
        //    List<Dictionary<string, HashSet<string>>> producersAndActorsDic = new List<Dictionary<string, HashSet<string>>>();
        //    Dictionary<string, HashSet<string>> producers = new Dictionary<string, HashSet<string>>();
        //    producersAndActorsDic.Add(producers);
        //    Dictionary<string, HashSet<string>> actors = new Dictionary<string, HashSet<string>>();
        //    producersAndActorsDic.Add(actors);
        //    string[] actorsAndProducersString;
        //    string initialCode = "tt0000001";
        //    HashSet<string> producersList = new HashSet<string>();
        //    HashSet<string> actorsSet = new HashSet<string>();
        //    string actor;
        //    string producer;
        //    while ((line = file.ReadLine()) != null)
        //    {
        //        actorsAndProducersString = line.Split('\t');
        //        if (actorsAndProducersString[0] != initialCode && actorsAndProducersString[0] != "tconst")
        //        {
        //            producersAndActorsDic[0].Add(initialCode, producersList);
        //            producersAndActorsDic[1].Add(initialCode, actorsSet);
        //            initialCode = actorsAndProducersString[0];
        //            producersList = new HashSet<string>();
        //            actorsSet = new HashSet<string>();
        //        }
        //        if (line.Contains("director") || line.Contains("producer"))
        //        {
        //            initialCode = actorsAndProducersString[0];

        //            staffDictionary.TryGetValue(actorsAndProducersString[2], out producer);

        //            producersList.Add(producer);
        //        }
        //        if (line.Contains("self") || line.Contains("actor") || line.Contains("actress"))
        //        {
        //            initialCode = actorsAndProducersString[0];

        //            staffDictionary.TryGetValue(actorsAndProducersString[2], out actor);

        //            actorsSet.Add(actor);
        //        }
        //    }
        //    return producersAndActorsDic;
        //}

        private static Dictionary<string, HashSet<string>> findProducers(string path, Dictionary<string, string> staffDictionary)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, HashSet<string>> producersDic = new Dictionary<string, HashSet<string>>();
            string[] producersString;
            string initialCode = "tt0000001";
            HashSet<string> producersList = new HashSet<string>();
            string producer;
            while ((line = file.ReadLine()) != null)
            {
                producersString = line.Split('\t');
                if (producersString[0] != initialCode && producersString[0] != "tconst")
                {
                    producersDic.Add(initialCode, producersList);
                    initialCode = producersString[0];
                    producersList = new HashSet<string>();
                }
                if (line.Contains("director") || line.Contains("producer"))
                {
                    initialCode = producersString[0];

                    staffDictionary.TryGetValue(producersString[2], out producer);

                    producersList.Add(producer);
                }

            }
            return producersDic;
        }

        private static Dictionary<string, HashSet<string>> findActors(string path, Dictionary<string, string> staffDictionary)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, HashSet<string>> actorsDic = new Dictionary<string, HashSet<string>>();
            string[] actorsString;
            string initialCode = "tt0000001";
            HashSet<string> actorsSet = new HashSet<string>();
            string actor;
            while ((line = file.ReadLine()) != null)
            {
                actorsString = line.Split('\t');
                if (actorsString[0] != initialCode && actorsString[0] != "tconst")
                {
                    actorsDic.Add(initialCode, actorsSet);
                    initialCode = actorsString[0];
                    actorsSet = new HashSet<string>();
                }
                if (line.Contains("self") || line.Contains("actor") || line.Contains("actress"))
                {
                    initialCode = actorsString[0];

                    staffDictionary.TryGetValue(actorsString[2], out actor);

                    actorsSet.Add(actor);
                }
            }
            return actorsDic;
        }

        private static Dictionary<string, string> findRating(string path)
        {
            string[] line = File.ReadAllLines(path);
            Dictionary<string, string> ratingDic = new Dictionary<string, string>();
            string[] ratingString;
            for (int i = 1; i < line.Length; i++)
            {
                ratingString = line[i].Split('\t');
                ratingDic.Add(ratingString[0], ratingString[1]);
            }
            return ratingDic;
        }

        //THIS IS THE DICTIONARY THAT CONSISTS OF KEY: MOVIECODE, VALUE: LIST OF MOVIENAMES: NAME OF THE MOVIE + LANGUAGE OF THAT MOVIE
        //private static Dictionary<string, List<MovieNames>> createDictionary1(string path)
        //{
        //    string[] line = File.ReadAllLines(path);
        //    Dictionary<string, List<MovieNames>> movieDic = new Dictionary<string, List<MovieNames>>();
        //    string[] movieString;
        //    string initialCode = "tt0000001";
        //    List<MovieNames> movieList = new List<MovieNames>();
        //    for(int i = 1; i<line.Length; i++) {
        //        movieString = line[i].Split('\t');
        //        if (movieString[0] != initialCode)
        //        {
        //            movieDic.Add(initialCode, movieList);
        //            initialCode = movieString[0];
        //            movieList = new List<MovieNames>();
        //        }
        //        if (movieString[3].ToLower() == "ru" || movieString[3].ToLower() == "us" || movieString[3].ToLower() == "suhh")
        //        {
        //            initialCode = movieString[0];
        //            MovieNames film = new MovieNames(movieString[2], movieString[3]);
        //            movieList.Add(film);
        //        }
        //    }
        //    return movieDic;
        //}

        private static Dictionary<string, List<MovieNames>> createDictionary1(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, List<MovieNames>> movieDic = new Dictionary<string, List<MovieNames>>();
            string[] movieString;
            string initialCode = "tt0000001";
            List<MovieNames> movieList = new List<MovieNames>();
            while ((line = file.ReadLine()) != null)
            {
                movieString = line.Split('\t');
                if (movieString[0] != initialCode && movieString[0] != "titleId")
                {
                    movieDic.Add(initialCode, movieList);
                    initialCode = movieString[0];
                    movieList = new List<MovieNames>();
                }
                if (movieString[3].ToLower() == "ru" || movieString[3].ToLower() == "us" || movieString[3].ToLower() == "suhh")
                {
                    initialCode = movieString[0];
                    MovieNames film = new MovieNames(movieString[2], movieString[3]);
                    movieList.Add(film);
                }
            }
            return movieDic;
        }


        private static Dictionary<string, string> createDictionary2(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            Dictionary<string, string> staffDic = new Dictionary<string, string>();
            string[] staffString;
            while ((line = file.ReadLine()) != null)
            {
                staffString = line.Split('\t');
                staffDic.Add(staffString[0], staffString[1]);
            }
            return staffDic;
        }

        private static string extractCode(string path, string movieName)
        {
            string code = "not exist";
            StreamReader file = new StreamReader(path);
            string[] codePath;
            string line;
            while ((line = file.ReadLine()) != null)
            {
                codePath = line.Split('\t');
                if (codePath[2].ToLower() == movieName)
                {
                    code = codePath[0];
                    return code;
                };
            }
            return code;
        }
    }
}
