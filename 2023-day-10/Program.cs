namespace _2023_day_10;

internal class Program
{
    static void Main(string[] args)
    {
        IList<string> data = GetPuzzleInput();

        PipeMap map = new(data);
    }

    static IList<string> GetPuzzleInput()
    {
        string file = Path.Combine(Environment.CurrentDirectory, "puzzle-input.txt");
        using StreamReader sr = new StreamReader(file);
        List<string> input = [];

        while (!sr.EndOfStream)
        {
            input.Add(sr.ReadLine()!);
        }

        return input;
    }

}
