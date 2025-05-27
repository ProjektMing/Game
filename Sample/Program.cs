namespace Sample;

/// <summary>
/// 应用的入口函数（无参）
/// </summary>
internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Hello, World!");
        using var game = new Game();
        game.Run();
    }
}