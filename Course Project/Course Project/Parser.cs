using System;
using System.Collections.Generic;
using System.Drawing;
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
                ' ', '\t', '\n', '\r', '/', '-', '.', '?', '!', ')',
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
                progressBar.Value = (i + 1) * 100 / numberOfLines;
                if (NotEmotionalWords.CheckWordForEmotional(noPunctuationStrings[i]))
                    wordsList.Add(noPunctuationStrings[i]);
            }
            return (new ArgsForTransact(richTextBox, wordsList));
            //wordsList = wordsList.Distinct().ToList();
        }

        public static void GetWordsChances(Object argsForTransact)
        {
            ThreadPool.SetMaxThreads(1,1);
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
                        //ColorWord(new ArgsForColor(wordInfo, richTextBox, word));
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ColorWord),
                            new ArgsForColor(wordInfo, richTextBox, word));
                        
                        positiveSum += wordInfo.GoodChance;
                        negativeSum += wordInfo.BadChance;
                        break;
                    }
                }
            }
            Thread.Sleep(1000);
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

        public static void ColorWord(Object argsForColor)
        {
            ArgsForColor args = (ArgsForColor) argsForColor;
            WordInfo wordInfo = args.WordInfo;
            RichTextBox richTextBox = args.RichTextBox;
            String word = args.Word;
            int index = 0;
            while ((index = richTextBox.Text.IndexOf(word, index)) != -1)
            {
                if (wordInfo.GoodChance > wordInfo.BadChance)
                {
                        richTextBox.Select(index, word.Length);
                        richTextBox.SelectionBackColor = Color.Green;
                        index += word.Length;
                }
                else
                {
                    if (wordInfo.GoodChance < wordInfo.BadChance)
                    {
                            richTextBox.Select(index, word.Length);
                            richTextBox.SelectionBackColor = Color.Red;
                            index += word.Length;
                    }
                    else
                    {
                            richTextBox.Select(index, word.Length);
                            richTextBox.SelectionBackColor = Color.Gray;
                            index += word.Length;
                    }
                }
            }
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
            Text = richTextBox.Text;
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
            Text = richTextBox.Text;
            RichTextBox = richTextBox;
            WordsList = wordsList;
        }
    }

    public class ArgsForColor
    {
        public WordInfo WordInfo { get; set; }
        public RichTextBox RichTextBox { get; set; }
        public String Word { get; set; }

        public ArgsForColor(WordInfo wordInfo, RichTextBox richTextBox, string word)
        {
            WordInfo = wordInfo;
            RichTextBox = richTextBox;
            Word = word;
        }
    }
}