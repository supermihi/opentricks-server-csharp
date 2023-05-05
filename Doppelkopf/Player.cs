namespace Doppelkopf;

public enum Player {
    Player1 = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3
}

public static class PlayerExtensions {

    public static Player Skip(this Player player, int positions) => (Player)(((int)(player) + positions) % 4);
}