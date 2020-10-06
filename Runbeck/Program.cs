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
                while(!File.Exists(path))
                {
                    Console.WriteLine("File cannot be found.  Please enter in a valid path.");
                    path = Console.ReadLine();
                }

                
                Console.WriteLine("Is the file format CSV (comma-separated values) or TSV (tab-separated values)?");
                var format = Console.ReadLine().Trim(); //"TSV";
                while(format.ToUpper()!= "TSV" && format.ToUpper() != "CSV")
                {
                    Console.WriteLine("You must type in TSV or CSV");
                    format = Console.ReadLine().Trim();
                }

                Console.WriteLine("How many fields should each record contain?");
                int i;
                string numOfFields = Console.ReadLine().Trim();  //3
                while(!Int32.TryParse(numOfFields, out i))
                {
                    Console.WriteLine("You must enter a number");
                    numOfFields = Console.ReadLine().Trim();
                }
               

                ReadData(path, format, int.Parse(numOfFields));

            }
            catch (Exception ex)
            {
                Console.Write("An error has occured: " + ex.Message, ex);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }
        /// <summary>
        /// Parse the input file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="numOfFields"></param>
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

                Console.WriteLine("End processing.  Press enter to close...");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                throw ex;
            }

           
        }

        /// <summary>
        /// This will write success and error files if records exist to
        /// the bin/outputfiles directory
        /// </summary>
        /// <param name="results"></param>
        /// <param name="successOrErrorFile"></param>
        /// <param name="format"></param>
        /// <returns></returns>
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
