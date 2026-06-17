using System;

namespace ChessEngine
{
    public static class PST
    {
        // 1. Pawns want to push forward and control the center.
        public static readonly int[] Pawns = {
            // Rank 1 (a1...h1)
             0,  0,  0,  0,  0,  0,  0,  0,
            // Rank 2
             5, 10, 10,-20,-20, 10, 10,  5,
            // Rank 3
             5, -5,-10,  0,  0,-10, -5,  5,
            // Rank 4
             0,  0,  0, 20, 20,  0,  0,  0,
            // Rank 5
             5,  5, 10, 25, 25, 10,  5,  5,
            // Rank 6
            10, 10, 20, 30, 30, 20, 10, 10,
            // Rank 7
            50, 50, 50, 50, 50, 50, 50, 50,
            // Rank 8
             90,  90,  90,  90,  90,  90,  90, 90
        };

        // 2. Knights are terrible on the edges, excellent in the center.
        public static readonly int[] Knights = {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        };

        // 3. Bishops want long diagonals.
        public static readonly int[] Bishops = {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
        };

        // 4. Rooks want open files and the 7th rank.
        public static readonly int[] Rooks = {
             0,  0,  0,  0,  0,  0,  0,  0,
             5, 10, 10, 10, 10, 10, 10,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
             0,  0,  0,  5,  5,  0,  0,  0
        };

        // 5. Queens generally stay back early on.
        public static readonly int[] Queens = {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  0,  0,-10,
             -5,  0,  5,  5,  5,  5,  0, -5,
              0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };

        // 6. King Safety (Midgame) - Hide behind pawns in the corners.
        public static readonly int[] KingMidgame = {
             20, 30, 10,  0,  0, 10, 30, 20,
             20, 20,  0,  0,  0,  0, 20, 20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30
        };

        // Helper to get the correct score based on piece type, color, and square
        public static int GetScore(int pieceType, int square)
        {
            // Assuming your piece indices:
            // 0=WP, 1=WN, 2=WB, 3=WR, 4=WQ, 5=WK
            // 6=BP, 7=BN, 8=BB, 9=BR, 10=BQ, 11=BK

            bool isBlack = pieceType > 5;
            int normalizedType = isBlack ? pieceType - 6 : pieceType;

            // Flipping the board for Black using XOR. 
            // 56 in binary is 111000. XORing by 56 elegantly mirrors rank 8 to rank 1, 7 to 2, etc.
            int index = isBlack ? square ^ 56 : square;

            return normalizedType switch
            {
                0 => Pawns[index],
                1 => Knights[index],
                2 => Bishops[index],
                3 => Rooks[index],
                4 => Queens[index],
                5 => KingMidgame[index],
                _ => 0
            };
        }
    }
}