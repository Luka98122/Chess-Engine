using System;

namespace ChessEngine
{
    public static class Bot
    {
        private const int Infinity = 2000000;

        // Simple inline MVV-LVA calculation
        public static int ScoreMove(Move m)
        {
            if (m.IsCapture)
            {
                // LVA: Subtracting the attacker's value means a Pawn (100) 
                // scores 900, while a Queen (900) scores 100.
                return 1000 - Math.Abs(Board.vals[m.PieceType]);
            }
            return 0;
        }

        private static int QuiescenceSearch(Board b, int alpha, int beta) //shallow check at the end of depth
        {
            // 1. "Stand Pat" Evaluation
            // If our position is already good enough without making any captures, 
            // we can establish a baseline score.
            int standPat = b.GetBoardEval();
            standPat = b.SideToMove == 0 ? standPat : -standPat;

            // Fail-hard beta cutoff
            if (standPat >= beta)
            {
                return beta;
            }

            // Update alpha if standing pat is better than our current alpha
            if (standPat > alpha)
            {
                alpha = standPat;
            }

            // 2. Generate Moves
            Span<Move> moves = stackalloc Move[218];
            int moveCount = allMoves.GenerateAllLegalMoves(b, moves, b.SideToMove);

            // Simple selection sort to bring the most promising captures to the front
            for (int i = 0; i < moveCount - 1; i++)
            {
                int bestIndex = i;
                int bestScore = ScoreMove(moves[i]);

                for (int j = i + 1; j < moveCount; j++)
                {
                    int score = ScoreMove(moves[j]);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestIndex = j;
                    }
                }

                if (bestIndex != i)
                {
                    // Swap
                    Move temp = moves[i];
                    moves[i] = moves[bestIndex];
                    moves[bestIndex] = temp;
                }
            }

            // 3. Search ONLY Captures
            for (int i = 0; i < moveCount; i++)
            {
                // Skip quiet moves. In a fully optimized engine, you would write a separate 
                // GenerateCaptureMoves method to avoid generating quiet moves entirely.
                if (!moves[i].IsCapture) continue;

                b.MakeMove(moves[i]);

                // Recursively call QS instead of regular Search
                int score = -QuiescenceSearch(b, -beta, -alpha);

                b.UnmakeMove();

                if (score >= beta)
                {
                    return beta; // Opponent has a refutation, prune this branch
                }
                if (score > alpha)
                {
                    alpha = score; // We found a better capture sequence
                }
            }

            return alpha;
        }

        public static Move Think(Board b, int depth)
        {
            Span<Move> moves = stackalloc Move[218];
            int moveCount = allMoves.GenerateAllLegalMoves(b, moves, b.SideToMove);

            // Fallback if no moves exist
            if (moveCount == 0) return default;

            Move bestMove = moves[0];
            int bestScore = -Infinity;
            int alpha = -Infinity;
            int beta = Infinity;

            for (int i = 0; i < moveCount; i++)
            {
                b.MakeMove(moves[i]);

                // Negate the score and swap bounds for the next player
                int score = -Search(b, depth - 1, -beta, -alpha);

                b.UnmakeMove();

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = moves[i];
                }

                if (score > alpha)
                {
                    alpha = score;
                }
            }

            return bestMove;
        }

        private static int Search(Board b, int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                // Instead of stopping blindly, ensure the position is stable
                return QuiescenceSearch(b, alpha, beta);
            }

            Span<Move> moves = stackalloc Move[218];
            int moveCount = allMoves.GenerateAllLegalMoves(b, moves, b.SideToMove);

            if (moveCount == 0)
            {
                int state = b.GetBoardState();
                if (state == 2) return 0; // Stalemate
                return -150000 - depth;   // Prioritize quicker checkmates
            }

            int bestScore = -Infinity;

            // Simple selection sort to bring the most promising captures to the front
            for (int i = 0; i < moveCount - 1; i++)
            {
                int bestIndex = i;
                int bestScore2 = ScoreMove(moves[i]);

                for (int j = i + 1; j < moveCount; j++)
                {
                    int score = ScoreMove(moves[j]);
                    if (score > bestScore2)
                    {
                        bestScore2 = score;
                        bestIndex = j;
                    }
                }

                if (bestIndex != i)
                {
                    // Swap
                    Move temp = moves[i];
                    moves[i] = moves[bestIndex];
                    moves[bestIndex] = temp;
                }
            }

            for (int i = 0; i < Math.Min(moveCount,10); i++)
            {
                b.MakeMove(moves[i]);
                int score = -Search(b, depth - 1, -beta, -alpha);
                b.UnmakeMove();

                if (score >= beta)
                {
                    return beta; // Fail-high cutoff: fail early because opponent won't allow this branch
                }
                if (score > bestScore)
                {
                    bestScore = score;
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }

            return bestScore;
        }
    }
}