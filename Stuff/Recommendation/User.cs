﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stuff.StuffMath.Structures;
using Stuff.StuffMath;

namespace Stuff.Recommendation
{
    public class User
    {
        public string Name { get; }

        public Dictionary<string, double> Ratings { get; }

        public User(string name, params (string film, double rating)[] ratings)
        {
            Name = name;
            Ratings = new Dictionary<string, double>();
            foreach (var (film, rating) in ratings)
                Ratings[film] = rating;
        }

        public User(string name, Dictionary<string, double> ratings)
        {
            Name = name;
            Ratings = ratings;
        }

        public double Mean()
        {
            return Ratings.Values.Sum(d => d) / Ratings.Count;
        }

        public double Mean(IEnumerable<string> films)
        {
            var result = 0d;
            var intersection = Ratings.Keys.Intersect(films);
            foreach (var film in intersection)
                result += Ratings[film];
            return result / intersection.Count();
        }

        public double Variance()
        {
            return Math.Sqrt(Ratings.Values.Sum(d => Math.Pow(d - Mean(), 2)) / Ratings.Count);
        }

        public double Variance(IEnumerable<string> films)
        {
            var result = 0d;
            var mean = Mean(films);
            var intersection = Ratings.Keys.Intersect(films);
            foreach (var film in intersection)
                result += Math.Pow(Ratings[film] - mean, 2);
            return Math.Sqrt(result / intersection.Count());
        }

        public Dictionary<string, double> Standardize()
        {
            var result = new Dictionary<string, double>();
            foreach (var r in Ratings)
                result.Add(r.Key, (r.Value - Mean()) / Variance());
            return result;
        }

        public Dictionary<string, double> Standardize(IEnumerable<string> films)
        {
            var result = new Dictionary<string, double>();
            foreach (var film in Ratings.Keys.Intersect(films))
                result.Add(film, (Ratings[film] - Mean(films)) / Variance(films));
            return result;
        }

        public double ESimilarity(User u)
        {
            var d = 0d;
            var films = Ratings.Keys.Intersect(u.Ratings.Keys);
            foreach(var film in films)
                d += Math.Pow(Ratings[film] - u.Ratings[film], 2);
            return 1 / (1 + Math.Sqrt(d));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="film">Remove this film before correlating.</param>
        /// <returns></returns>
        public double PCorrelation(User u, string film = "")
        {
            var films = Ratings.Keys.Where(k => k != film).Intersect(u.Ratings.Keys);
            var u1 = Standardize(films);
            var u2 = u.Standardize(films);
            var d = 0d;
            foreach (var f in films)
                d += u1[f] * u2[f];
            return d / films.Count();
        }
    }
}
