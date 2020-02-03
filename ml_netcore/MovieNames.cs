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
    class MovieNames
    {
        public string filmName;
        public string filmLang;

        public MovieNames(string filmName, string filmLang)
        {
            this.filmName = filmName;
            this.filmLang = filmLang;
        }
        public string FilmName
        {
            get => this.filmName;
        }
        public string FilmLang
        {
            get => this.filmLang;
        }
        public override string ToString()
        {
            return "Film is: " + this.filmName + ", Language " + this.filmLang;
        }
    }
}
