using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fib
{
    public class FileBundler
    {
        private readonly string[] excludedDirectories = { "bin", "obj", "debug", "release",".vs" };
        private readonly string currentDirectory = Directory.GetCurrentDirectory();

        public async Task BundleFilesAsync(string language, string output, bool note, string sort, bool removeEmptyLines, string author)
        {
            try
            {
                var files = GetFiles(language, sort);

                if (files.Length == 0)
                {
                    Console.WriteLine("No files found for the specified languages.");
                    return;
                }

                await WriteToFileAsync(files, output, note, removeEmptyLines, author);
                Console.WriteLine("Files bundled successfully into " + output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An error occurred while creating the bundled file. {ex.Message}");
            }
        }

        private string[] GetFiles(string language, string sort)
        {
            var files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories)
                .Where(f => !excludedDirectories.Any(d => IsPathInDirectory(f, d)))
                .Where(f => language == "all" || language.Split(' ').Any(ext => f.EndsWith($".{ext.Trim()}")))
                .ToArray();

            if (sort == "type")
            {
                files = files.OrderBy(f => Path.GetExtension(f)).ThenBy(f => f).ToArray();
            }
            else
            {
                files = files.OrderBy(f => f).ToArray();
            }

            return files;
        }

        private bool IsPathInDirectory(string filePath, string directoryName)
        {
            return filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                           .Contains(directoryName, StringComparer.OrdinalIgnoreCase);
        }

        private async Task WriteToFileAsync(string[] files, string output, bool note, bool removeEmptyLines, string author)
        {
            await using (var fs = new FileStream(output, FileMode.Create))
            await using (var writer = new StreamWriter(fs))
            {
                if (!string.IsNullOrWhiteSpace(author))
                {
                    await writer.WriteLineAsync($"// Author: {author}");
                }

                foreach (var file in files)
                {
                    try
                    {
                        var fileContent = await File.ReadAllLinesAsync(file);

                        if (note)
                        {
                            await writer.WriteLineAsync($"// Source: {Path.GetRelativePath(currentDirectory, file)}");
                        }
                        foreach (var line in fileContent)
                        {
                            if (removeEmptyLines && string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }
                            await writer.WriteLineAsync(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: An error occurred while processing the file '{file}'. {ex.Message}");
                    }
                }
            }
        }
    }
}