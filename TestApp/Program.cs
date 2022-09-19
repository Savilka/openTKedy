namespace TestApp;

public class Program {
    static void Main(string[] args) {
        using var game = new Game(400, 400, "GG");
        game.Run();
    }
}