using System;
using System.IO;
using Convertor.Json;
using Convertor.Xml;

namespace Convertor
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }

            bool toXml = args[0] == "xml";

            if (!toXml && args[0] != "json")
            {
                PrintUsage();
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"The input file '{ args[1] }' does not exist.");
                return;
            }

            Convert(toXml, args[1], args[2]);
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Convertor usage:");
            Console.WriteLine("convertor.exe {xml|json} {inputFile} {outputFile}");
            Console.WriteLine("- First argument is the output format");
            Console.WriteLine("- Input file has to exist");
            Console.WriteLine("- Output file will be overwritten or created");
            Console.WriteLine("- Output file can be replaced with '-' to print to the standard output");
        }

        public static void Convert(bool toXml, string sourceFile, string destinationFile)
        {
            string source = null;
            using(StreamReader reader = new StreamReader(new FileStream(sourceFile, FileMode.Open)))
                source = reader.ReadToEnd();

            Stream destination = null;
            try
            {
                destination = Console.OpenStandardOutput();
                
                if (destinationFile != "-")
                    destination = new FileStream(destinationFile, FileMode.Create);

                // conversions

                if (IsXmlFile(sourceFile) && !toXml)
                    ConvertXmlToJson(source, destination);

                if (IsJsonFile(sourceFile) && toXml)
                    ConvertJsonToXml(source, destination);

                // pretty-prints

                if (IsXmlFile(sourceFile) && toXml)
                    PrettifyXml(source, destination);

                if (IsJsonFile(sourceFile) && !toXml)
                    PrettifyJson(source, destination);

                // unknown input
                if (!IsXmlFile(sourceFile) && !IsJsonFile(sourceFile))
                    Console.WriteLine("Input file is not Json nor Xml (unknown file extension).");

                destination.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("There was a problem with file access:\n");
                Console.WriteLine(e);
            }
            catch (Lemon.ParsingException e)
            {
                Console.WriteLine($"Cannot parse the file '{ sourceFile }':\n");
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occure:\n");
                Console.WriteLine(e);
            }
            finally
            {
                if (destination != null)
                    destination.Close();
            }
        }

        public static bool IsXmlFile(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".xml";
        }

        public static bool IsJsonFile(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".json";
        }

        public static void ConvertXmlToJson(string xml, Stream json)
        {
            var xmlAst = ReadXml(xml);
            var jsonAst = ToJson.Convert(xmlAst);
            
            using (StreamWriter writer = new StreamWriter(json))
                jsonAst.Stringify(writer, Json.StringifyOptions.PrettyPrint);
        }

        public static void ConvertJsonToXml(string json, Stream xml)
        {
            var jsonAst = ReadJson(json);
            var xmlAst = ToXml.Convert(jsonAst);
            
            using (StreamWriter writer = new StreamWriter(xml))
                xmlAst.Stringify(writer, Xml.StringifyOptions.PrettyPrint);
        }

        public static void PrettifyXml(string xml, Stream output)
        {
            var xmlAst = ReadXml(xml);
            
            using (StreamWriter writer = new StreamWriter(output))
                xmlAst.Stringify(writer, Xml.StringifyOptions.PrettyPrint);
        }

        public static void PrettifyJson(string json, Stream output)
        {
            var jsonAst = ReadJson(json);
            
            using (StreamWriter writer = new StreamWriter(output))
                jsonAst.Stringify(writer, Json.StringifyOptions.PrettyPrint);
        }

        public static XmlElement ReadXml(string xml)
        {
            var parser = XP.Element().CreateAbstractValuedParser();
            parser.Parse(xml);

            if (!parser.Success)
                throw parser.Exception;

            return parser.Value;
        }

        public static JsonEntity ReadJson(string json)
        {
            var parser = JP.Entity().CreateAbstractValuedParser();
            parser.Parse(json);

            if (!parser.Success)
                throw parser.Exception;

            return parser.Value;
        }
    }
}
