using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Words;
using System.Threading.Tasks;

namespace Course_Project
{
    public class Parser
    {
        public static Stemmer Stemmer = new Stemmer();
        public static WordsCollection WordsCollection =
            XML_loader.DeserializeCollection(WordsCollection, @"C:\Users\Amargo\Documents\GitHub\Course_Project\Course Project\WordsList1.xml");
        public static char[] separators = {
                ' ', '\t', '\n', '\r', '/',  '.', '?', '!', ')',
                '(', ',', ':', ';'
            };
        public static ArgsForTransact GetWordsFromText(Object parserArgsObj)
        {
            ArgsForAnalysisThread args = (ArgsForAnalysisThread) parserArgsObj;
            ProgressBar progressBar = args.ProgressBar;
            RichTextBox richTextBox = args.RichTextBox;
            String text = args.Text;
            List<string> wordsList = new List<string>();
            text = NormalizeText(text);
            string[] noPunctuationStrings = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int numberOfLines = noPunctuationStrings.Length;
            for (int i = 0; i < numberOfLines; i++)
            {
                //progressBar.Value = (i + 1) * 100 / numberOfLines;
                if (NotEmotionalWords.CheckWordForEmotional(noPunctuationStrings[i]))
                    wordsList.Add(noPunctuationStrings[i]);
            }
            return (new ArgsForTransact(richTextBox, wordsList));
            //wordsList = wordsList.Distinct().ToList();
        }

        public static void GetWordsChances(Object argsForTransact)
        {
            ArgsForTransact args = (ArgsForTransact) argsForTransact;
            RichTextBox richTextBox = args.RichTextBox;
            List<string> wordsList = args.WordsList;
            decimal positiveSum = new decimal();
            decimal negativeSum = new decimal();
            List<Thread> colorThreads = new List<Thread>();
            foreach (string word in wordsList)
            {   
                foreach (WordInfo wordInfo in WordsCollection.WordInfos)
                {
                    if (Stemmer.Stem(word) == wordInfo.Word)
                    {
                        positiveSum += wordInfo.GoodChance;
                        negativeSum += wordInfo.BadChance;
                        break;
                    }
                }
            }
            MessageBox.Show("Postive - " + positiveSum + "\nNegative - " + negativeSum);
        }


        public static void GetAnalysisResult(Object argsForAnalysis)
        {
            GetWordsChances(GetWordsFromText(argsForAnalysis));
        }
        public static string NormalizeText(string text)
        {
            text = text.ToLower();
            //удаляем цифры
            text=Regex.Replace(text,"[0-9]", "");
            //Заменяем char 160 и 151 - html-мусор из кинопоиска
            text = text.Replace(((char) 160).ToString(), " ").Replace(((char) 151).ToString(), " ");
            //Удаляем все названия в кавычках
            text = Regex.Replace(text, (char) 171 + "{1,}" + (char) 187, "");
            return text;
        }
    }

    //Аргументы для запуска потока
    public class ArgsForAnalysisThread
    {
        public string Text { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public RichTextBox RichTextBox { get; set; }

        public ArgsForAnalysisThread(ProgressBar progressBar, RichTextBox richTextBox)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke((MethodInvoker) delegate() { Text = richTextBox.Text; });
            }
            else
            {
                Text = richTextBox.Text;
            }
            ProgressBar = progressBar;
            RichTextBox = richTextBox;
        }
    }

    //Аргументы для передачи после разделения на слова в определение ценности слов
    public class ArgsForTransact
    {
        public string Text { get; set; }
        public RichTextBox RichTextBox { get; set; }
        public List<string> WordsList { get; set; }
        public ArgsForTransact(RichTextBox richTextBox, List<string> wordsList)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke((MethodInvoker)delegate() { Text = richTextBox.Text; });
            }
            else
            {
                Text = richTextBox.Text;
            }
            RichTextBox = richTextBox;
            WordsList = wordsList;
        }
    }
}