# AutoGlobalUsing

[![it](https://img.shields.io/badge/Leggi_in_italiano!-red.svg)](https://github.com/Davidencho/AutoGlobalUsing/blob/master/docs/README.it.md)


This small project automates the process of reorganizing `using` directives within C# projects (version 10 and above).

The idea is simple: instead of having `using` directives scattered across various source files, the tool collects them into a single file, improving the readability and maintainability of the code.

The tool can be executed **on a single C# project or recursively on an entire folder of projects**. For each detected project, a separate `GlobalUsing` file is generated, moving all `using` directives to the new file and keeping the structure clean.

### Features
- **Support for a single project or project folder**: You can analyze a single project or perform a recursive scan of a folder to process all C# projects within it.
- **Customization of the output file name**: You can specify in the *appsettings.json* file the final name of the file to be produced.
- **Automatic removal of existing `GlobalUsing` files**: You can use a configurable regular expression in *appsettings.json* to find and remove all existing `GlobalUsing` files in the project, ensuring complete cleanliness before generating the new file, while preserving any individual global `using` directives already present in these files.
- **Handling of `using` aliases**: You can decide whether to keep or remove `using` aliases during the reorganization based on your needs.

### Usage

In the releases section, you can find the precompiled program available for download in a zip file. You can choose to run it directly and input the folder path via the console by simply following the instructions, or you can run it by passing the name of the folder to analyze as an argument.

```
AutoGlobalUsing.exe MY_PATH true
```

The second parameter, which you see as `true`, is `verbose`, and when set to true (it will be true by default), it will display all the `using` directives being moved to the new file for all projects.

--- 

If you found this project helpful, feel free to buy me a coffee!

<a href="https://www.buymeacoffee.com/Davidencho" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174"></a>
