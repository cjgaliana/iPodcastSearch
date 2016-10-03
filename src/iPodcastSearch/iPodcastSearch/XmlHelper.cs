namespace iPodcastSearch
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public static class XmlHelper
    {
        public static string Serialize(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T Deserialize<T>(string objectData)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;

            using (var reader = new StringReader(objectData))
            {
                result = (T) serializer.Deserialize(reader);
            }

            return result;
        }
    }
}