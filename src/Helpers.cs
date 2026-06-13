using System;

namespace ChessEngine
{
    public static class EngineHelpers
    {
        // A simple test function
        public static void PrintStatus()
        {
            Console.WriteLine("Function called successfully from EngineHelpers.cs!");
        }

        // A function that interacts with an existing Board class
        public static void AnalyzePosition(Board b)
        {
            string color = b.SideToMove == 0 ? "White" : "Black";
            Console.WriteLine($"\n[Analysis] It is currently {color}'s turn to move.");
        }

        // Helper to set the standard 64-bit hexadecimal starting positions
        public static void InitializeStartingPosition(Board b)
        {
            // --- White Pieces ---
            b.Pieces[0] = 0x000000000000FF00; // Pawns (Rank 2)
            b.Pieces[1] = 0x0000000000000042; // Knights (b1, g1)
            b.Pieces[2] = 0x0000000000000024; // Bishops (c1, f1)
            b.Pieces[3] = 0x0000000000000081; // Rooks (a1, h1)
            b.Pieces[4] = 0x0000000000000008; // Queen (d1)
            b.Pieces[5] = 0x0000000000000010; // King (e1)

            // --- Black Pieces ---
            b.Pieces[6] = 0x00FF000000000000; // Pawns (Rank 7)
            b.Pieces[7] = 0x4200000000000000; // Knights (b8, g8)
            b.Pieces[8] = 0x2400000000000000; // Bishops (c8, f8)
            b.Pieces[9] = 0x8100000000000000; // Rooks (a8, h8)
            b.Pieces[10] = 0x0800000000000000; // Queen (d8)
            b.Pieces[11] = 0x1000000000000000; // King (e8)
            
            b.SideToMove = 0; // White's turn to move
        }

        // Helper to render the board state in the terminal
        public static void RenderBoard(Board b)
        {
            // Indices match the bitboard array (0-5 White, 6-11 Black)
            // Uppercase for White, Lowercase for Black
            char[] pieceChars = { 'P', 'N', 'B', 'R', 'Q', 'K', 'p', 'n', 'b', 'r', 'q', 'k' };

            Console.WriteLine("  +-----------------+");
            
            // Loop backwards from Rank 8 down to Rank 1 so the board prints right-side up
            for (int rank = 7; rank >= 0; rank--)
            {
                Console.Write($"{rank + 1} | ");
                
                for (int file = 0; file < 8; file++)
                {
                    int square = rank * 8 + file;
                    char printChar = '.'; // Default empty square

                    // Check all 12 bitboards to see if any piece is occupying this square
                    for (int pieceType = 0; pieceType < 12; pieceType++)
                    {
                        // Bitwise AND to check if the specific square's bit is a 1
                        if ((b.Pieces[pieceType] & (1UL << square)) != 0)
                        {
                            printChar = pieceChars[pieceType];
                            break;
                        }
                    }
                    Console.Write(printChar + " ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("  +-----------------+");
            Console.WriteLine("    a b c d e f g h");
        }
    }
}