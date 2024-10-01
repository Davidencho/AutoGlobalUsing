namespace AutoGlobalUsing
{
    internal class AutoGlobalUsing
    {
        private static async Task Main(string[] args)
        {
            string path;
            bool isVerbose = true;
            try
            {
                if (args.Length > 0)
                {
                    path = args[0];

                    if (args.Length > 1 && !bool.TryParse(args[1], out isVerbose))
                        isVerbose = true;
                }
                else
                {
                    Console.Write("Insert a folder's path: ");
                    path = Console.ReadLine() ?? throw new ArgumentNullException(nameof(path).ToUpper());
                    Console.WriteLine("Verbose? (Y/n)");
                    isVerbose = !char.ToLower(Console.ReadKey().KeyChar).Equals('n');
                }
                await new AutoGlobalUsingManager(path, isVerbose).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Terminated. Press a key to exit...");
            Console.ReadKey();
        }
    }
}
