using System;
using System.Diagnostics;
using System.Numerics;

namespace ChessEngine
{
    public static class Zobrist
    {
        public static readonly ulong[,] Pieces = new ulong[12, 64];
        public static readonly ulong[] Castling = new ulong[16];
        public static readonly ulong[] EnPassant = new ulong[64];
        public static readonly ulong SideToMove;

        static Zobrist()
        {
            // mora da bude fiksiran seed
            Random rnd = new Random(6767);
            byte[] buffer = new byte[8];

            ulong NextRandom()
            {
                rnd.NextBytes(buffer);
                return BitConverter.ToUInt64(buffer, 0);
            }

            for (int p = 0; p < 12; p++)
                for (int sq = 0; sq < 64; sq++)
                    Pieces[p, sq] = NextRandom();

            for (int i = 0; i < 16; i++)
                Castling[i] = NextRandom();

            for (int i = 0; i < 64; i++)
                EnPassant[i] = NextRandom();

            SideToMove = NextRandom();
        }
    }
    public struct TTEntry
    {
        public ulong Key;
        public int Depth;
        public int Score;
        public int Flag; // 0 = Exact,1 = Alpha (gornja granica), 2 = Beta (donja granica)
        public Move BestMove;
    }

    public static class TT
    {
        private const int Size = 0x1000000;
        private const int SizeMask = Size - 1;
        public static TTEntry[] Entries = new TTEntry[Size];
        public static int CacheHits = 0;
        private static int filledEntries = 0;
        public static void Store(ulong key, int depth, int score, int flag, Move bestMove)
        {
            int index = (int)(key & SizeMask);
            ref TTEntry existing = ref Entries[index];

            if (existing.Key == 0)
            {
                int currentFilled = Interlocked.Increment(ref filledEntries);
            }

            if (existing.Key != key || depth >= existing.Depth)
            {
                existing = new TTEntry { Key = key, Depth = depth, Score = score, Flag = flag, BestMove = bestMove };
            }
        }

        public static bool TryProbe(ulong key, int depth, int alpha, int beta, out int score, out Move bestMove)
        {
            score = 0;
            bestMove = default;
            int index = (int)(key & SizeMask);
            TTEntry entry = Entries[index];

            if (entry.Key == key)
            {
                bestMove = entry.BestMove;
                if (entry.Depth >= depth)
                {
                    if (entry.Flag == 0) { score = entry.Score; CacheHits++; return true; }
                    else if (entry.Flag == 1 && entry.Score <= alpha) { score = alpha; CacheHits++; return true; }
                    else if (entry.Flag == 2 && entry.Score >= beta) { score = beta; CacheHits++; return true; }
                }
            }
            return false;
        }
    }
    public static class Bot
    {
        private const int Infinity = 2000000;
        public static Dictionary<(Board board, int depth, int a, int b, int c, int sideToMove), int> cache = new();
        public static int ScoreMove(Move m)
        {
            int score = 0;

            if (m.IsPromotion)
            {
                score += 9000;
            }

            if (m.IsCapture && m.CapturedPieceType >= 0)
            {
                int victimValue = Math.Abs(Board.vals[m.CapturedPieceType]);
                int attackerValue = Math.Abs(Board.vals[m.PieceType]);
                score += victimValue * 10 - attackerValue;
            }

            return score;
        }

        private static int QuiescenceSearch(Board b, int alpha, int beta)
        {
            int standPat = b.GetBoardEval(includeHangingPieces: false);
            standPat = b.SideToMove == 0 ? standPat : -standPat;

            if (standPat >= beta)
            {
                return beta;
            }

            if (standPat > alpha)
            {
                alpha = standPat;
            }

            Span<Move> moves = stackalloc Move[218];
            int moveCount = allMoves.GenerateAllLegalCaptures(b, moves, b.SideToMove);

            Span<int> moveScores = stackalloc int[moveCount];
            for (int i = 0; i < moveCount; i++)
            {
                moveScores[i] = ScoreMove(moves[i]);
            }

            for (int i = 0; i < moveCount - 1; i++)
            {
                int bestIndex = i;
                for (int j = i + 1; j < moveCount; j++)
                {
                    if (moveScores[j] > moveScores[bestIndex])
                        bestIndex = j;
                }
                if (bestIndex != i)
                {
                    Move tempMove = moves[i];
                    moves[i] = moves[bestIndex];
                    moves[bestIndex] = tempMove;
                    int tempScore = moveScores[i];
                    moveScores[i] = moveScores[bestIndex];
                    moveScores[bestIndex] = tempScore;
                }
            }

            for (int i = 0; i < moveCount; i++)
            {
                b.MakeMove(moves[i]);
                int score = -QuiescenceSearch(b, -beta, -alpha);
                b.UnmakeMove();

                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }

            return alpha;
        }

        public static Move Think(Board b, int targetDepth, int topX)
        {
            Span<Move> moves = stackalloc Move[218];
            int moveCount = allMoves.GenerateAllLegalMoves(b, moves, b.SideToMove);
            if (moveCount == 0) return default;

            Span<int> moveScores = stackalloc int[moveCount];
            for (int i = 0; i < moveCount; i++)
            {
                moveScores[i] = ScoreMove(moves[i]);
            }

            // Selection sort
            for (int i = 0; i < moveCount - 1; i++)
            {
                int bestIndex = i;
                for (int j = i + 1; j < moveCount; j++)
                {
                    if (moveScores[j] > moveScores[bestIndex])
                    {
                        bestIndex = j;
                    }
                }

                if (bestIndex != i)
                {
                    // Swap moves
                    Move tempMove = moves[i];
                    moves[i] = moves[bestIndex];
                    moves[bestIndex] = tempMove;

                    // Swap scores
                    int tempScore = moveScores[i];
                    moveScores[i] = moveScores[bestIndex];
                    moveScores[bestIndex] = tempScore;
                }
            }


            Move bestMoveThisTurn = moves[0];

            //iterative deepening
            var sw = Stopwatch.StartNew();
            for (int currentDepth = 1; currentDepth <= targetDepth; currentDepth++)
            {
                int alpha = -Infinity;
                int beta = Infinity;
                int bestScore = -Infinity;
                Move bestMoveThisDepth = moves[0];

                for (int i = 0; i < moveCount; i++)
                {
                    b.MakeMove(moves[i]);
                    int score = -Search(b, currentDepth - 1, -beta, -alpha);
                    b.UnmakeMove();

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMoveThisDepth = moves[i];
                    }
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }

                bestMoveThisTurn = bestMoveThisDepth;
                Debug.WriteLine($"[PERF] Depth {currentDepth}/{targetDepth} took {sw.Elapsed.TotalSeconds:F2}s");
                sw.Restart();
            }

            return bestMoveThisTurn;
        }

        public static int Search(Board b, int depth, int alpha, int beta)
        {
            int originalAlpha = alpha;

            if (TT.TryProbe(b.ZobristKey, depth, alpha, beta, out int ttScore, out Move ttBestMove))
            {
                return ttScore;
            }

            if (depth <= 0)
            {
                return QuiescenceSearch(b, alpha, beta);
            }

            int kingType = b.SideToMove == 0 ? 5 : 11;
            int kingSquare = BitOperations.TrailingZeroCount(b.Pieces[kingType]);
            bool inCheck = b.IsSquareAttacked(kingSquare, 1 - b.SideToMove);

            if (depth >= 3 && !inCheck)
            {
                int savedSTM = b.SideToMove;
                int savedEP = b.EnPassantSquare;
                ulong savedKey = b.ZobristKey;

                b.SideToMove = 1 - b.SideToMove;
                b.EnPassantSquare = -1;
                b.ZobristKey ^= Zobrist.SideToMove;
                if (savedEP != -1) b.ZobristKey ^= Zobrist.EnPassant[savedEP];

                int nullScore = -Search(b, depth - 1 - 3, -beta, -beta + 1); //NMP

                b.SideToMove = savedSTM;
                b.EnPassantSquare = savedEP;
                b.ZobristKey = savedKey;

                if (nullScore >= beta)
                    return beta;
            }

            Span<Move> moves = stackalloc Move[256]; // bolje nego na heapu
            int moveCount = allMoves.GenerateAllLegalMoves(b, moves, b.SideToMove);

            if (moveCount == 0)
            {
                if (inCheck)
                    return -30000 - depth;
                return 0;
            }

            if (ttBestMove.FromSquare != 0 || ttBestMove.ToSquare != 0)
            {
                for (int i = 0; i < moveCount; i++)
                {
                    if (moves[i].FromSquare == ttBestMove.FromSquare &&
                        moves[i].ToSquare == ttBestMove.ToSquare)
                    {
                        Move temp = moves[0];
                        moves[0] = moves[i];
                        moves[i] = temp;
                        break;
                    }
                }
            }

            Span<int> moveScores = stackalloc int[moveCount];
            for (int i = 0; i < moveCount; i++)
            {
                moveScores[i] = ScoreMove(moves[i]);
            }

            for (int i = 1; i < moveCount - 1; i++)
            {
                int bestIndex = i;
                for (int j = i + 1; j < moveCount; j++)
                {
                    if (moveScores[j] > moveScores[bestIndex])
                        bestIndex = j;
                }
                if (bestIndex != i)
                {
                    Move tempMove = moves[i];
                    moves[i] = moves[bestIndex];
                    moves[bestIndex] = tempMove;
                    int tempScore = moveScores[i];
                    moveScores[i] = moveScores[bestIndex];
                    moveScores[bestIndex] = tempScore;
                }
            }

            int bestScore = -int.MaxValue;
            Move bestMoveInNode = moves[0];

            for (int i = 0; i < moveCount; i++)
            {
                b.MakeMove(moves[i]);

                int score;
                if (i >= 4 && depth >= 3 && !moves[i].IsCapture && !moves[i].IsPromotion)
                {
                    score = -Search(b, depth - 1 - 2, -alpha - 1, -alpha);
                    if (score > alpha)
                        score = -Search(b, depth - 1, -beta, -alpha);
                }
                else
                {
                    score = -Search(b, depth - 1, -beta, -alpha);
                }

                b.UnmakeMove();

                if (score >= beta)
                {
                    TT.Store(b.ZobristKey, depth, beta, 2, moves[i]);
                    return beta;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveInNode = moves[i];
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }

            int flag = 0;
            if (bestScore <= originalAlpha)
            {
                flag = 1;
            }

            TT.Store(b.ZobristKey, depth, bestScore, flag, bestMoveInNode);
            return bestScore;
        }
    }
}