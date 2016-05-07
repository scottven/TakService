using System;
using System.Collections.Generic;
using System.Linq;

namespace TakEngine.Notation
{
    public enum NotatedMoveType
    {
        Place,
        MoveStack
    }

    public class MoveNotation
    {
        public int StoneType;
        public NotatedMoveType MoveType;
        public BoardPosition Position;
        public int PickupCount = 0;
        public int? UnstackDirection;
        public List<int> UnstackCounts = null;
        public string Text;

        private MoveNotation()
        {
        }

        public bool Compare(MoveNotation target)
        {
            if (MoveType != target.MoveType)
                return false;
            if (MoveType == NotatedMoveType.MoveStack)
            {
                if (UnstackDirection != target.UnstackDirection ||
                    Position != target.Position ||
                    UnstackCounts.Count != target.UnstackCounts.Count)
                    return false;
                for (int i = 0; i < UnstackCounts.Count; i++)
                    if (UnstackCounts[i] != target.UnstackCounts[i])
                        return false;
                return true;
            }
            else
            {
                return StoneType == target.StoneType && Position == target.Position;
            }
        }

        public static bool TryParse(string s, out MoveNotation notated)
        {
            notated = null;

            // minimum move notation length
            if (s.Length < 2)
                return false;

            notated = new MoveNotation();
            notated.Text = s;

            // try to find a direction
            int dirIndex = s.IndexOfAny(Direction.DirName);
            if (dirIndex >= 0)
            {
                notated.UnstackDirection = Array.IndexOf(Direction.DirName, s[dirIndex]);
                notated.MoveType = NotatedMoveType.MoveStack;

                // direction must be immediately preceded by position
                int positionStart = dirIndex - 2;
                if (positionStart < 0)
                    return false;
                if (!TryParsePosition(s, out notated.Position, index: positionStart))
                    return false;

                // Pickup count is optional if we're only picking up a single stone
                if (positionStart == 0)
                    notated.PickupCount = 1;

                // Otherwise we need to be able to parse the number of stones picked up
                else if (!int.TryParse(s.Substring(0, positionStart), out notated.PickupCount))
                    return false;

                // Paranoia
                if (notated.PickupCount < 0 || notated.PickupCount > 8)
                    return false;

                notated.UnstackCounts = new List<int>(4);
                int remaining = notated.PickupCount;
                for (int i = dirIndex + 1; i < s.Length; i++)
                {
                    int count = (int)(s[i] - '0');
                    if (count < 0 || count > remaining)
                        return false;
                    notated.UnstackCounts.Add(count);
                    remaining -= count;

                    if (remaining == 0)
                    {
                        // optional F, S, or C to indicate the top of the stack at the end of the move
                        if (i < s.Length - 2)
                            // too many characters at the end of the string!
                            return false;
                        else if (i == s.Length - 2)
                        {
                            var c = s[i + 1];
                            if (c != 'F' && c != 'S' && c != 'C')
                                // unrecognized stone type, don't know what to do with this
                                return false;
                        }
                        break;
                    }
                }

                // Drop count is optional if we're only moving a single stone
                if (notated.UnstackCounts.Count == 0 && remaining > 0)
                {
                    notated.UnstackCounts.Add(remaining);
                    remaining = 0;
                }
                if (remaining != 0)
                    return false;
                return true;
            }
            else
            {
                // no direction found, this must be a placement move
                notated.MoveType = NotatedMoveType.Place;

                if (s.Length > 3)
                    return false;
                if (s.StartsWith("S"))
                    notated.StoneType = Piece.Stone_Standing;
                else if (s.StartsWith("C"))
                    notated.StoneType = Piece.Stone_Cap;
                else if (s.StartsWith("F") || s.Length == 2)
                    notated.StoneType = Piece.Stone_Flat;
                else
                    return false;

                if (!TryParsePosition(s, out notated.Position, index: s.Length - 2))
                    return false;
                return true;
            }
        }

        public static bool TryParsePosition(string s, out BoardPosition pos, int index = 0)
        {
            pos = new BoardPosition();
            if (s.Length - index < 2)
                return false;
            pos.X = s[index] - (int)'a';
            if (pos.X < 0 || pos.X > 8)
                return false;
            pos.Y = s[index + 1] - (int)'1';
            if (pos.Y < 0 || pos.Y > 8)
                return false;
            return true;
        }

        /// <summary>
        /// Attempt to match this notation to one of the legal moves provided
        /// </summary>
        /// <param name="legalMoves">List of moves that are legal in the current position</param>
        /// <returns>Returns the move which matches this notation, or null if no match was found</returns>
        public IMove MatchLegalMove(IList<IMove> legalMoves)
        {
            if (MoveType == NotatedMoveType.Place)
            {
                return legalMoves
                    .OfType<PlacePieceMove>()
                    .Where(x => x.Pos == Position)
                    .Where(x => Piece.GetStone(x.PieceID) == StoneType)
                    .FirstOrDefault();
            }
            else
            {
                // Get a list of all unstack moves which begin at this position and pick up the # of stones we're looking for
                var unstackMoves = legalMoves
                    .OfType<PickupAndPlaceMove>()
                    .Where(x => x.PickUpMove.Position == Position
                        && x.PickUpMove.PickUpCount == PickupCount
                        && x.UnstackDirection == UnstackDirection);

                // Examine each candidate and compare the number of stones placed in each cell
                foreach (var candidate in unstackMoves)
                {
                    var candidateUnstackCounts = candidate.GetUnstackCounts();
                    int comparingAt = 0;
                    bool matchedAllUnstackCounts = true;
                    foreach (var comparing in candidateUnstackCounts)
                    {
                        if (comparing != UnstackCounts[comparingAt++])
                        {
                            matchedAllUnstackCounts = false;
                            break;
                        }
                    }
                    if (!matchedAllUnstackCounts)
                        continue;
                    if (comparingAt != UnstackCounts.Count)
                        continue;
                    return candidate;
                }
                return null;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
