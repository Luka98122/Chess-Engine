using System;
using System.Collections.Generic;
using System.Numerics;
using ChessEngine;
using static ChessEngine.EngineHelpers;
using static ChessEngine.KnightMoveGenerator;


namespace ChessEngine
{
    public struct Move
    {
        public int FromSquare;
        public int ToSquare;
        public int PieceType;

        public bool IsCapture;
        public bool IsPromotion;
        public int PromotedPieceType;
        public bool IsEnPassant;
        public bool IsCastle;
        public int CapturedPieceType;

        public Move(int from, int to, int piece, bool isCapture = false)
        {
            FromSquare = from;
            ToSquare = to;
            PieceType = piece;
            IsCapture = isCapture;

            IsPromotion = false;
            PromotedPieceType = -1;
            IsEnPassant = false;
            IsCastle = false;
            CapturedPieceType = -1;
        }
    }

    public struct BoardStateInfo
    {
        public Move MoveMade;
        public int CapturedPieceType;
        public byte CastlingRights;
        public int EnPassantSquare;
        public int HalfMoveClock;
        public int GameType;
        public ulong ZobristKey;
        public ulong WhiteOccupancy;
        public ulong BlackOccupancy;
        public int PieceCount;
    }

    public class Board
    {
        // Indices 0-5: White (P, N, B, R, Q, K)
        // Indices 6-11: Black (P, N, B, R, Q, K)
        public ulong[] Pieces = new ulong[12];

        public int SideToMove = 0; // 0 beli, 1 crni
        public byte CastlingRights = 15;
        public int EnPassantSquare = -1;
        public int HalfMoveClock = 0;
        public int GameType = 0; // 0 - Early, 1 - Mid, 2 - End
        public int PieceCount;
        public ulong WhiteOccupancy;
        public ulong BlackOccupancy;
        public ulong Occupied => WhiteOccupancy | BlackOccupancy;
        // 15 bianrno je 1111 - obe strane imaju oba prava
        public static readonly byte[] CastlingRightsMask = new byte[64] {
            13, 15, 15, 15, 12, 15, 15, 14,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15,
            7, 15, 15, 15, 3, 15, 15, 11 
        };
        private BoardStateInfo[] _stateHistory = new BoardStateInfo[2048];
        private int _historyPly = 0;

        public ulong ZobristKey;

        public ulong GenerateKey()
        {
            ulong key = 0;

            for (int p = 0; p < 12; p++)
            {
                ulong bitboard = Pieces[p];
                while (bitboard != 0)
                {
                    int sq = System.Numerics.BitOperations.TrailingZeroCount(bitboard);
                    key ^= Zobrist.Pieces[p, sq];
                    bitboard &= bitboard - 1; // Clear LSB
                }
            }

            if (SideToMove == 1) key ^= Zobrist.SideToMove;

            key ^= Zobrist.Castling[CastlingRights];

            if (EnPassantSquare != -1) key ^= Zobrist.EnPassant[EnPassantSquare];
            return key;
        }

        public void MakeMove(Move move)
        {
            if (_historyPly >= _stateHistory.Length)
            {
                Array.Resize(ref _stateHistory, _stateHistory.Length * 2);
            }

            ref BoardStateInfo state = ref _stateHistory[_historyPly++];
            state.CastlingRights = CastlingRights;
            state.MoveMade = move;
            state.HalfMoveClock = HalfMoveClock;
            state.EnPassantSquare = EnPassantSquare;
            state.GameType = GameType;
            state.CapturedPieceType = -1;
            state.ZobristKey = this.ZobristKey;
            state.WhiteOccupancy = WhiteOccupancy;
            state.BlackOccupancy = BlackOccupancy;
            state.PieceCount = PieceCount;

            byte oldCastlingRights = CastlingRights;
            int oldEP = EnPassantSquare; 
            ulong zobristDelta = 0;

            EnPassantSquare = -1;

            CastlingRights &= CastlingRightsMask[move.FromSquare];
            CastlingRights &= CastlingRightsMask[move.ToSquare];
            if (oldCastlingRights != CastlingRights)
            {
                zobristDelta ^= Zobrist.Castling[oldCastlingRights];
                zobristDelta ^= Zobrist.Castling[CastlingRights];
            }

            if (move.IsEnPassant)
            {
                int capturedPawnSquare = move.ToSquare + (SideToMove == 0 ? -8 : 8);
                int opponentPawnType = SideToMove == 0 ? 6 : 0;
                state.CapturedPieceType = opponentPawnType;
                Pieces[opponentPawnType] &= ~(1UL << capturedPawnSquare);
                zobristDelta ^= Zobrist.Pieces[opponentPawnType, capturedPawnSquare];
                if (SideToMove == 0) BlackOccupancy &= ~(1UL << capturedPawnSquare);
                else WhiteOccupancy &= ~(1UL << capturedPawnSquare);
                PieceCount--;
                HalfMoveClock = 0;
            }
            else if (move.IsCapture)
            {
                int startIndex = SideToMove == 0 ? 6 : 0;
                ulong targetBit = 1UL << move.ToSquare;

                for (int i = startIndex; i <= startIndex + 5; i++)
                {
                    if ((Pieces[i] & targetBit) != 0)
                    {
                        state.CapturedPieceType = i;
                        Pieces[i] &= ~targetBit;
                        zobristDelta ^= Zobrist.Pieces[i, move.ToSquare];
                        if (i < 6) WhiteOccupancy &= ~targetBit;
                        else BlackOccupancy &= ~targetBit;
                        break;
                    }
                }
                PieceCount--;
                HalfMoveClock = 0;
            }
            else if (move.PieceType == 0 || move.PieceType == 6)
            {
                HalfMoveClock = 0;
            }
            else
            {
                HalfMoveClock++;
            }

            Pieces[move.PieceType] &= ~(1UL << move.FromSquare);
            zobristDelta ^= Zobrist.Pieces[move.PieceType, move.FromSquare];
            if (move.PieceType < 6) WhiteOccupancy &= ~(1UL << move.FromSquare);
            else BlackOccupancy &= ~(1UL << move.FromSquare);

            if (move.IsPromotion)
            {
                int promoType = move.PromotedPieceType + 6 * SideToMove;
                Pieces[promoType] |= (1UL << move.ToSquare);
                zobristDelta ^= Zobrist.Pieces[promoType, move.ToSquare];
                if (promoType < 6) WhiteOccupancy |= (1UL << move.ToSquare);
                else BlackOccupancy |= (1UL << move.ToSquare);
            }
            else
            {
                Pieces[move.PieceType] |= (1UL << move.ToSquare);
                zobristDelta ^= Zobrist.Pieces[move.PieceType, move.ToSquare];
                if (move.PieceType < 6) WhiteOccupancy |= (1UL << move.ToSquare);
                else BlackOccupancy |= (1UL << move.ToSquare);
            }

            if ((move.PieceType == 0 || move.PieceType == 6) && !move.IsPromotion)
            {
                int fromRank = move.FromSquare / 8;
                int toRank = move.ToSquare / 8;
                if (Math.Abs(toRank - fromRank) == 2)
                {
                    EnPassantSquare = move.FromSquare + (SideToMove == 0 ? 8 : -8);
                }
            }

            if (move.IsCastle) //rokada
            {
                int rookType = SideToMove == 0 ? 3 : 9;

                if (move.ToSquare == 6) // beli kratka
                {
                    Pieces[rookType] &= ~(1UL << 7); Pieces[rookType] |= (1UL << 5);
                    zobristDelta ^= Zobrist.Pieces[rookType, 7]; zobristDelta ^= Zobrist.Pieces[rookType, 5];
                    if (SideToMove == 0) { WhiteOccupancy &= ~(1UL << 7); WhiteOccupancy |= (1UL << 5); }
                    else { BlackOccupancy &= ~(1UL << 7); BlackOccupancy |= (1UL << 5); }
                }
                else if (move.ToSquare == 2) // beli duga
                {
                    Pieces[rookType] &= ~(1UL << 0); Pieces[rookType] |= (1UL << 3);
                    zobristDelta ^= Zobrist.Pieces[rookType, 0]; zobristDelta ^= Zobrist.Pieces[rookType, 3];
                    if (SideToMove == 0) { WhiteOccupancy &= ~(1UL << 0); WhiteOccupancy |= (1UL << 3); }
                    else { BlackOccupancy &= ~(1UL << 0); BlackOccupancy |= (1UL << 3); }
                }
                else if (move.ToSquare == 62) // crni kratka
                {
                    Pieces[rookType] &= ~(1UL << 63); Pieces[rookType] |= (1UL << 61);
                    zobristDelta ^= Zobrist.Pieces[rookType, 63]; zobristDelta ^= Zobrist.Pieces[rookType, 61];
                    if (SideToMove == 0) { WhiteOccupancy &= ~(1UL << 63); WhiteOccupancy |= (1UL << 61); }
                    else { BlackOccupancy &= ~(1UL << 63); BlackOccupancy |= (1UL << 61); }
                }
                else if (move.ToSquare == 58) // crni duga
                {
                    Pieces[rookType] &= ~(1UL << 56); Pieces[rookType] |= (1UL << 59);
                    zobristDelta ^= Zobrist.Pieces[rookType, 56]; zobristDelta ^= Zobrist.Pieces[rookType, 59];
                    if (SideToMove == 0) { WhiteOccupancy &= ~(1UL << 56); WhiteOccupancy |= (1UL << 59); }
                    else { BlackOccupancy &= ~(1UL << 56); BlackOccupancy |= (1UL << 59); }
                }
            }

            // Early,mid,end - game state
            if (PieceCount <= 23)
                GameType = 1;
            if (PieceCount <= 7)
                GameType = 2;

            
            SideToMove = SideToMove == 0 ? 1 : 0;
            zobristDelta ^= Zobrist.SideToMove;

            if (oldEP != -1) zobristDelta ^= Zobrist.EnPassant[oldEP];
            if (EnPassantSquare != -1) zobristDelta ^= Zobrist.EnPassant[EnPassantSquare];

            ZobristKey = state.ZobristKey ^ zobristDelta;
        }

        public void UnmakeMove()
        {
            ref BoardStateInfo state = ref _stateHistory[--_historyPly];
            Move move = state.MoveMade;

            SideToMove = 1 - SideToMove;
            CastlingRights = state.CastlingRights;
            EnPassantSquare = state.EnPassantSquare;
            HalfMoveClock = state.HalfMoveClock;
            GameType = state.GameType;
            WhiteOccupancy = state.WhiteOccupancy;
            BlackOccupancy = state.BlackOccupancy;
            PieceCount = state.PieceCount;

            if (move.IsPromotion)
            {
                Pieces[move.PromotedPieceType + 6 * SideToMove] &= ~(1UL << move.ToSquare);
            }
            else
            {
                Pieces[move.PieceType] &= ~(1UL << move.ToSquare);
            }

            Pieces[move.PieceType] |= (1UL << move.FromSquare);

            if (state.CapturedPieceType != -1)
            {
                if (move.IsEnPassant)
                {
                    int capturedSquare = move.ToSquare + (SideToMove == 0 ? -8 : 8);
                    Pieces[state.CapturedPieceType] |= (1UL << capturedSquare);
                }
                else
                {
                    Pieces[state.CapturedPieceType] |= (1UL << move.ToSquare);
                }
            }

            if (move.IsCastle)
            {
                int rookType = SideToMove == 0 ? 3 : 9;

                if (move.ToSquare == 6) { Pieces[rookType] &= ~(1UL << 5); Pieces[rookType] |= (1UL << 7); } // g1
                else if (move.ToSquare == 2) { Pieces[rookType] &= ~(1UL << 3); Pieces[rookType] |= (1UL << 0); } // c1
                else if (move.ToSquare == 62) { Pieces[rookType] &= ~(1UL << 61); Pieces[rookType] |= (1UL << 63); } // g8
                else if (move.ToSquare == 58) { Pieces[rookType] &= ~(1UL << 59); Pieces[rookType] |= (1UL << 56); } // c8
            }
            this.ZobristKey = state.ZobristKey;
        }

        public bool IsSquareAttacked(int square, int attackerColor)
        {
            if (square < 0 || square > 63) return false;
            ulong friendlyPieces = attackerColor == 0 ? WhiteOccupancy : BlackOccupancy;
            ulong enemyPieces = attackerColor == 0 ? BlackOccupancy : WhiteOccupancy;
            ulong occupied = WhiteOccupancy | BlackOccupancy;
            ulong squareBB = 1UL << square;

            ulong knights = Pieces[attackerColor == 0 ? 1 : 7];
            if ((KnightMoveGenerator.KnightPreCalcs[square] & knights) != 0) return true;

            ulong kings = Pieces[attackerColor == 0 ? 5 : 11];
            if ((KingMoveGenerator.KingPreCalcs[square] & kings) != 0) return true;

            ulong pawns = Pieces[attackerColor == 0 ? 0 : 6];
            if (attackerColor == 0) //beli napada
            {
                if ((((squareBB & PawnMoveGenerator.NotFileA) >> 9) & pawns) != 0) return true;
                if ((((squareBB & PawnMoveGenerator.NotFileH) >> 7) & pawns) != 0) return true;
            }
            else //crni napada
            {
                if ((((squareBB & PawnMoveGenerator.NotFileA) << 7) & pawns) != 0) return true;
                if ((((squareBB & PawnMoveGenerator.NotFileH) << 9) & pawns) != 0) return true;
            }

            ulong bishopsQueens = Pieces[attackerColor == 0 ? 2 : 8] | Pieces[attackerColor == 0 ? 4 : 10];
            if (bishopsQueens != 0)
            {
                ulong bishopBlockers = occupied & BishopMoveGenerator.BishopMasks[square];
                int bMagicIndex = (int)((bishopBlockers * BishopMoveGenerator.BishopMagics[square]) >> (64 - BishopMoveGenerator.BishopRelevantBits[square]));
                if ((BishopMoveGenerator.BishopAttacks[square][bMagicIndex] & bishopsQueens) != 0) return true;
            }

            ulong rooksQueens = Pieces[attackerColor == 0 ? 3 : 9] | Pieces[attackerColor == 0 ? 4 : 10];
            if (rooksQueens != 0)
            {
                ulong rookBlockers = occupied & RookMoveGenerator.RookMasks[square];
                int rMagicIndex = (int)((rookBlockers * RookMoveGenerator.RookMagics[square]) >> (64 - RookMoveGenerator.RookRelevantBits[square]));
                if ((RookMoveGenerator.RookAttacks[square][rMagicIndex] & rooksQueens) != 0) return true;
            }

            return false;
        }

        public int GetBoardState()
        {
            Span<Move> moves = stackalloc Move[218]; // mnogo brze od heap memorije
            int legalMoveCount = allMoves.GenerateAllLegalMoves(this, moves, this.SideToMove);

            if (legalMoveCount > 0)
            {
                if (this.HalfMoveClock >= 100) return 2; // stalemate (pat)
                return -1; // normal
            }

            int kingPieceType = 5 + this.SideToMove * 6;
            int kingSquare = BitOperations.TrailingZeroCount(this.Pieces[kingPieceType]);
            int attackerColor = 1 - this.SideToMove;

            if (this.IsSquareAttacked(kingSquare, attackerColor))
            {
                return attackerColor; // attackerColor wins
            }
            return 2; // Stalemate (pat)
        }

        private int GetCheapestAttackerValue(int sq, int attackerColor, ulong occupied)
        {
            int pOffset = attackerColor == 0 ? 0 : 6;
            ulong targetBit = 1UL << sq;

            // Ako beli pijun napada sq onda mora biti na sq-7 ili sq-, sl. za crne
            ulong pawns = Pieces[pOffset + 0];
            ulong pawnAttackers = attackerColor == 0
                ? ((targetBit >> 7) & PawnMoveGenerator.NotFileA) | ((targetBit >> 9) & PawnMoveGenerator.NotFileH)
                : ((targetBit << 7) & PawnMoveGenerator.NotFileH) | ((targetBit << 9) & PawnMoveGenerator.NotFileA);
            if ((pawns & pawnAttackers) != 0) return 100;

            if ((KnightMoveGenerator.KnightPreCalcs[sq] & Pieces[pOffset + 1]) != 0) return 300;

            ulong bBlockers = occupied & BishopMoveGenerator.BishopMasks[sq];
            int bMagic = (int)((bBlockers * BishopMoveGenerator.BishopMagics[sq]) >> (64 - BishopMoveGenerator.BishopRelevantBits[sq]));
            ulong bAttacks = BishopMoveGenerator.BishopAttacks[sq][bMagic];
            if ((bAttacks & Pieces[pOffset + 2]) != 0) return 300;

            ulong rBlockers = occupied & RookMoveGenerator.RookMasks[sq];
            int rMagic = (int)((rBlockers * RookMoveGenerator.RookMagics[sq]) >> (64 - RookMoveGenerator.RookRelevantBits[sq]));
            ulong rAttacks = RookMoveGenerator.RookAttacks[sq][rMagic];
            if ((rAttacks & Pieces[pOffset + 3]) != 0) return 500;

            if (((bAttacks | rAttacks) & Pieces[pOffset + 4]) != 0) return 900;

            if ((KingMoveGenerator.KingPreCalcs[sq] & Pieces[pOffset + 5]) != 0) return 6767;

            return 99999; // Nije napadnut
        }

        public static readonly int[] vals = new int[] { 100, 300, 300, 500, 900, 6767, -100, -300, -300, -500, -900, -6767 }; 
        public int GetBoardEval(bool includeHangingPieces = true)
        {

            int score = 0;
            for (int pt = 0; pt < 12; pt++)
            {
                ulong bitboard = this.Pieces[pt];
                bool isBlack = pt > 5;

                int materialValue = Math.Abs(vals[pt]);

                while (bitboard != 0)
                {
                    int sq = System.Numerics.BitOperations.TrailingZeroCount(bitboard);

                    int positionalBonus = PST.GetScore(pt, sq);

                    int pieceScore = materialValue + positionalBonus;

                    if (isBlack)
                    {
                        score -= pieceScore;
                    }
                    else
                    {
                        score += pieceScore;
                    }

                    bitboard &= bitboard - 1;
                }
            }


            int wKingSquare = BitOperations.TrailingZeroCount(this.Pieces[5]);
            bool isWInCheck = this.IsSquareAttacked(wKingSquare, 1); //black attacking white king
            if (isWInCheck)
            {
                score -= 50; // Configurable, but putting in check is weighted to 50cp (half a pawn)
            }

            int bKingSquare = BitOperations.TrailingZeroCount(this.Pieces[11]);
            bool isBInCheck = this.IsSquareAttacked(bKingSquare, 0); //white attacking black king
            if (isBInCheck)
            {
                score += 50; // Configurable, but putting in check is weighted to 50cp (half a pawn)
            }

            if (includeHangingPieces)
            {
                ulong occupied = this.Occupied;

                for (int i = 0; i < 5; i++)
                {
                    ulong piecesIter = this.Pieces[i];
                    while (piecesIter != 0)
                    {
                        int sq = BitOperations.TrailingZeroCount(piecesIter);

                        int cheapestAttacker = GetCheapestAttackerValue(sq, 1, occupied);

                        if (cheapestAttacker != 99999)
                        {
                            bool isDefended = this.IsSquareAttacked(sq, 0);
                            int pieceValue = vals[i];

                            if (!isDefended || cheapestAttacker < pieceValue)
                            {
                                score -= pieceValue / 2;
                            }
                        }
                        piecesIter &= piecesIter - 1;
                    }
                }

                for (int i = 6; i < 11; i++)
                {
                    ulong piecesIter = this.Pieces[i];
                    while (piecesIter != 0)
                    {
                        int sq = BitOperations.TrailingZeroCount(piecesIter);

                        int cheapestAttacker = GetCheapestAttackerValue(sq, 0, occupied);

                        if (cheapestAttacker != 99999)
                        {
                            bool isDefended = this.IsSquareAttacked(sq, 1);
                            int pieceValue = Math.Abs(vals[i]); 
                            if (!isDefended || cheapestAttacker < pieceValue)
                            {
                                score -= vals[i] / 2;
                            }
                        }
                        piecesIter &= piecesIter - 1;
                    }
                }
            }

            return score;
        }

        public Board Clone()
        {
            Board copy = new Board();

            copy.Pieces = new ulong[this.Pieces.Length];
            Array.Copy(this.Pieces, copy.Pieces, this.Pieces.Length);

            copy.SideToMove = this.SideToMove;
            copy.CastlingRights = this.CastlingRights;
            copy.EnPassantSquare = this.EnPassantSquare;
            copy.HalfMoveClock = this.HalfMoveClock;
            copy.GameType = this.GameType;
            copy.ZobristKey = this.ZobristKey;
            copy.WhiteOccupancy = this.WhiteOccupancy;
            copy.BlackOccupancy = this.BlackOccupancy;
            copy.PieceCount = this.PieceCount;
            copy._historyPly = this._historyPly;
            Array.Copy(this._stateHistory, copy._stateHistory, this._stateHistory.Length);

            return copy;
        }

        public int GetPieceTypeAtSquare(int square)
        {
            ulong mask = 1UL << square;
            for (int i = 0; i < 12; i++)
                if ((Pieces[i] & mask) != 0)
                    return i;
            return -1;
        }

        public void ComputeInitialOccupancy()
        {
            WhiteOccupancy = Pieces[0] | Pieces[1] | Pieces[2] | Pieces[3] | Pieces[4] | Pieces[5];
            BlackOccupancy = Pieces[6] | Pieces[7] | Pieces[8] | Pieces[9] | Pieces[10] | Pieces[11];
            PieceCount = BitOperations.PopCount(WhiteOccupancy) + BitOperations.PopCount(BlackOccupancy);
            if (PieceCount <= 7) GameType = 2;
            else if (PieceCount <= 23) GameType = 1;
            else GameType = 0;
        }
    }
}

