using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fib
{

    public class ResponseFile
    {
        public void CreateResponseFile(string outputFile)
        {
            try
            {
                var responseContent = GetResponseContent();
                File.WriteAllText(outputFile, responseContent);
                Console.WriteLine($"Response file created at {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An error occurred while creating the response file. {ex.Message}");
            }
        }

        private string GetResponseContent()
        {
            Console.Write("Please enter the programming languages (space-separated or 'all'): ");
            string languages = Console.ReadLine();

            Console.Write("Please enter the output file name/path: ");
            string output = Console.ReadLine();

            Console.Write("Include source code notes? (y/n): ");
            bool note = Console.ReadLine().Trim().ToLower() == "y";

            Console.Write("Sort files by 'name' or 'type': ");
            string sort = Console.ReadLine().Trim().ToLower();

            Console.Write("Remove empty lines? (y/n): ");
            bool removeEmptyLines = Console.ReadLine().Trim().ToLower() == "y";

            Console.Write("Author name: ");
            string author = Console.ReadLine();

            string responseContent = $"bundle --language {languages} --output {output}";
            if (note) responseContent += " --note";
            responseContent += $" --sort {sort}";
            if (removeEmptyLines) responseContent += " --remove-empty-lines";
            if (!string.IsNullOrWhiteSpace(author)) responseContent += $" --author {author}";

            return responseContent;
        }
    }
}
