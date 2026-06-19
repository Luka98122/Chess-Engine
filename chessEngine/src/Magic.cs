using System;
using static ChessEngine.EngineHelpers;
using static ChessEngine.KnightMoveGenerator;
using static ChessEngine.RookMoveGenerator;
using System.Numerics;


namespace ChessEngine
{
    public static class MagicFinder
    {
        private static Random rnd = new Random();

        private static ulong GetRandomUlong()
        {
            byte[] buffer = new byte[8];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        //Bitwise & tri random broja dosta smaji broj jedinica
        // (Otp svaki osmi bit je jedinica)
        private static ulong GetSparseRandomUlong()
        {
            return GetRandomUlong() & GetRandomUlong() & GetRandomUlong();
        }

        public static ulong FindRookMagic(int square, int relevantBits)
        {
            int permutationCount = 1 << relevantBits;
            ulong[] blockers = new ulong[permutationCount];
            ulong[] attacks = new ulong[permutationCount];

            ulong mask = RookMoveGenerator.CreateRookMask(square);

            ulong blockerPattern = 0;
            int i = 0;
            do
            {
                blockers[i] = blockerPattern;
                attacks[i] = RookMoveGenerator.CalculateNaiveRookAttacks(square, blockerPattern);
                i++;
                blockerPattern = (blockerPattern - mask) & mask;
            } while (blockerPattern != 0);

            for (int attempt = 0; attempt < 100000000; attempt++)
            {
                ulong magic = GetSparseRandomUlong();

                if (BitOperations.PopCount((mask * magic) & 0xFF00000000000000UL) < 6)
                    continue;

                ulong[] usedAttacks = new ulong[permutationCount];
                bool[] isUsed = new bool[permutationCount];
                bool fail = false;

                for (int j = 0; j < permutationCount; j++)
                {
                    int magicIndex = (int)((blockers[j] * magic) >> (64 - relevantBits));

                    if (!isUsed[magicIndex])
                    {
                        isUsed[magicIndex] = true;
                        usedAttacks[magicIndex] = attacks[j];
                    }
                    else if (usedAttacks[magicIndex] != attacks[j])
                    {
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {
                    return magic; //radi
                }
            }

            Console.WriteLine($"Failed to find rook magic for square {square}");
            return 0UL;
        }

        public static void GenerateAllRookMagics()
        {
            Console.WriteLine("public static readonly ulong[] RookMagics = new ulong[64] {");
            for (int square = 0; square < 64; square++)
            {
                ulong mask = RookMoveGenerator.CreateRookMask(square);
                int relevantBits = BitOperations.PopCount(mask);

                ulong magic = FindRookMagic(square, relevantBits);
                Console.WriteLine($"    0x{magic:X16}UL, // Square {square}");
            }
            Console.WriteLine("};");
        }

        public static ulong FindBishopMagic(int square, int relevantBits)
        {
            int permutationCount = 1 << relevantBits;
            ulong[] blockers = new ulong[permutationCount];
            ulong[] attacks = new ulong[permutationCount];

            ulong mask = BishopMoveGenerator.CreateBishopMask(square);

            ulong blockerPattern = 0;
            int i = 0;
            do
            {
                blockers[i] = blockerPattern;
                attacks[i] = BishopMoveGenerator.CalculateNaiveBishopAttacks(square, blockerPattern);
                i++;
                blockerPattern = (blockerPattern - mask) & mask;
            } while (blockerPattern != 0);

            for (int attempt = 0; attempt < 100000000; attempt++)
            {
                ulong magic = GetSparseRandomUlong();

                if (BitOperations.PopCount((mask * magic) & 0xFF00000000000000UL) < 6)
                    continue;

                ulong[] usedAttacks = new ulong[permutationCount];
                bool[] isUsed = new bool[permutationCount];
                bool fail = false;

                for (int j = 0; j < permutationCount; j++)
                {
                    int magicIndex = (int)((blockers[j] * magic) >> (64 - relevantBits));

                    if (!isUsed[magicIndex])
                    {
                        isUsed[magicIndex] = true;
                        usedAttacks[magicIndex] = attacks[j];
                    }
                    else if (usedAttacks[magicIndex] != attacks[j])
                    {
                         
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {
                    return magic; //Jupi
                }
            }

            Console.WriteLine($"Failed to find bishop magic for square {square}");
            return 0UL;
        }

        public static void GenerateAllBishopMagics()
        {
            Console.WriteLine("public static readonly ulong[] BishopMagics = new ulong[64] {");
            for (int square = 0; square < 64; square++)
            {
                ulong mask = BishopMoveGenerator.CreateBishopMask(square);
                int relevantBits = BitOperations.PopCount(mask);

                ulong magic = FindBishopMagic(square, relevantBits);
                Console.WriteLine($"    0x{magic:X16}UL, // Square {square}");
            }
            Console.WriteLine("};");
        }
    }
}