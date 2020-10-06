using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Runbeck
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
               

                Console.WriteLine("Where is file located?");
                var path = Console.ReadLine(); //"Users/collincouch/Projects/Runbeck/Runbeck/test.tsv";

                
                Console.WriteLine("Is the file format CSV (comma-separated values) or TSV (tab-separated values)?");
                var format = Console.ReadLine(); //"TSV";

                Console.WriteLine("How many fields should each record contain?");
                var numOfFields = Console.ReadLine();  //3

                ReadData(path, format, int.Parse(numOfFields));

            }
            catch (Exception ex)
            {
                Console.Write("An error has occured: " + ex.Message, ex);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }

        private static void ReadData(string fileName, string fileType, int numOfFields)
        {

            Console.WriteLine("Start Processing : " + DateTime.Now);

            //I'm chosing to use a dictonary data structure
            //to store success and error records
            //sacraficing space for time.
            Dictionary<int, string> successData = new Dictionary<int, string>();
            Dictionary<int, string> errorData = new Dictionary<int, string>();

            char sep;
           // string extension = Path.GetExtension(fileName);
           // string result = fileName.Substring(0, fileName.Length - extension.Length);

            try
            {

                if (fileType.ToUpper() == "CSV")
                    sep = ',';
                else if (fileType.ToUpper() == "TSV")
                    sep = '\t';
                else
                    throw new Exception("Unknown file type");

                int errorCounter= 1;//starting error dictionary key at 1
                int successCounter = 1;//starting success dictionary key at 1

                using (StreamReader reader = new StreamReader(fileName))
                {
                    reader.ReadLine(); // skipping header

                    while (!reader.EndOfStream)
                    {

                        var line = reader.ReadLine();
                        var values = line.Split(sep);//seperate on sep

                        //make sure number of fields is correct for each line
                        if (values.Length != numOfFields)
                        {
                            //add to error record to dictionary
                            errorData.Add(errorCounter++, line);
                        }
                        else
                        {
                            //add to success record dictionary
                            successData.Add(successCounter++, line);
                        }

                    }
                }

                //Write success file using the successData dictionary
                if(successData.Count>=1)
                {
                    string succOutPutFilePath = WriteFile(successData, "Success", fileType);
                    Console.WriteLine(string.Format("Success file located at:{0}", succOutPutFilePath));

                }

                //Write error file using the errorData dictionary
                if(errorData.Count>=1)
                {
                    string errOutPutFilePath = WriteFile(errorData, "Error", fileType);
                    Console.WriteLine(string.Format("Error file located at:{0}", errOutPutFilePath));

                }

                Console.WriteLine("Press enter to close...");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                throw ex;
            }

           
        }


        private static string WriteFile(Dictionary<int,string> results,string successOrErrorFile, string format)
        {
            string outPutDirectory = Directory.GetCurrentDirectory() + "\\OutputFiles";
            long timeStamp = DateTime.UtcNow.ToFileTimeUtc();//timestamp to make it unique
            
            string fileName = string.Format("{0}_{1}.{2}",
                successOrErrorFile.ToUpper()=="SUCCESS"?"Success":"Error",
                timeStamp,format);
            string outPutFilePath = string.Format("{0}\\{1}", outPutDirectory, fileName);

            try
            {
                //Some Linq to query the file data
                String file = String.Join(
                    Environment.NewLine,
                    results.Select(d => $"{d.Value}")
                );
                File.WriteAllText(outPutFilePath, file);

                return outPutFilePath;
                
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured writing the output file", ex);
            }

        }
    }
}
