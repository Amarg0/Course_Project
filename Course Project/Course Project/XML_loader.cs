using System;
using System.IO;
using System.Xml.Serialization;

namespace Course_Project
{
    public class XML_loader
    {
        public static void SerializeCollection(WordsCollection wordsCollection, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WordsCollection),
                new Type[]{typeof(WordInfo)});
            FileStream fileStream = new FileStream(path,FileMode.Create);
            xmlSerializer.Serialize(fileStream,wordsCollection);
            fileStream.Close();
        }

        public static WordsCollection DeserializeCollection(WordsCollection wordsCollection, string path)
        {
            wordsCollection = new WordsCollection();
            var xmlSerializer = new XmlSerializer(typeof(WordsCollection),
                new Type[]{typeof(WordInfo)});
            var fileStream = new FileStream(path,FileMode.Open);
            wordsCollection = (WordsCollection) xmlSerializer.Deserialize(fileStream);
            fileStream.Close();
            return wordsCollection;
        }
    }
}