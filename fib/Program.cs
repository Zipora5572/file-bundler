using System.CommandLine;
using System.Collections.Generic;
using fib;

class Program
{
    static Option<T> CreateOption<T>(string name, string alias, string description, bool isRequired = false)
    {
        var option = new Option<T>(name, description)
        {
            IsRequired = isRequired
        };
        option.AddAlias(alias);
        return option;
    }

    static async Task Main(string[] args)
    {
        var fileBundler = new FileBundler();
        var responseFile = new ResponseFile();

        var bundleCommand = new Command("bundle", "Bundle code files to a single file");
        var languageOption = CreateOption<string>("--language", "-l", "List of programming languages (or 'all')", true);
        var outputOption = CreateOption<string>("--output", "-o", "Name of the output bundle file", true);
        var noteOption = CreateOption<bool>("--note", "-n", "Include source code as a comment");
        var sortOption = CreateOption<string>("--sort", "-s", "Sort files by name or type");
        var removeEmptyLinesOption = CreateOption<bool>("--remove-empty-lines", "-r", "Remove empty lines from code");
        var authorOption = CreateOption<string>("--author", "-a", "Author of the bundle");
        var outputRsp = CreateOption<string>("--output-file", "-or", "Name of the response file");

        var options = new List<Option>
        {
            languageOption,
            outputOption,
            noteOption,
            sortOption,
            removeEmptyLinesOption,
            authorOption
        };

        foreach (var option in options)
        {
            bundleCommand.Add(option);
        }

        bundleCommand.SetHandler(async (string language, string output, bool note, string sort, bool removeEmptyLines, string author) =>
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                Console.WriteLine("Language is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine("Output is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(sort))
            {
                sort = "name";
            }
           else if (sort != "name" && sort != "type")
            {
                Console.WriteLine("Invalid sort option. Please use 'name' or 'type'.");
                return;
            }

            await fileBundler.BundleFilesAsync(language, output, note, sort, removeEmptyLines, author);
        },
        languageOption, outputOption, noteOption, sortOption, removeEmptyLinesOption, authorOption);

        var createRspCommand = new Command("create-rsp", "Create a response file with ready command") { outputRsp };

        createRspCommand.SetHandler((string outputFile) =>
        {
            responseFile.CreateResponseFile(outputFile);
        }, outputRsp);

        var rootCommand = new RootCommand("Root command for File Bundler CLI")
        {
            bundleCommand,
            createRspCommand
        };

        await rootCommand.InvokeAsync(args);
    }
}