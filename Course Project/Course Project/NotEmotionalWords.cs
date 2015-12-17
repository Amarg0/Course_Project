using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Course_Project.Properties;
using Words;

namespace Course_Project
{
    class NotEmotionalWords
    {
        public static List<string> NotEmotionaList = new List<string>();
        public static Stemmer Stemmer = new Stemmer();
        public static void GetNotEmoWords()
        {
            char[] separators = new[] {'\n', ' ', '\r'};
            string[] notEmotionalWords = Resources.NotEmotionalWords.Split(separators,
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in notEmotionalWords)
            {
                NotEmotionaList.Add(word);
            }
        }
            
        public static bool CheckWordForEmotional(string word)
        {
            GetNotEmoWords();
            if (NotEmotionaList.IndexOf(word) > 0 || Stemmer.Stem(word) == "" ||
                (Stemmer.Stem(word).ToCharArray().Length == 1) ||
                NotEmotionaList.IndexOf(Stemmer.Stem(word)) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
