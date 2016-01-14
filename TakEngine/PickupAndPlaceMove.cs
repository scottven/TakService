using System.Collections.Generic;

namespace TakEngine
{
    /// <summary>
    /// Represents picking up 1 or more pieces from a stack, putting them into our hand, and then placing them in other cells
    /// </summary>
    public class PickupAndPlaceMove : ChainMove
    {
        /// <summary>
        /// Creates a shallow copy by copying the contained moves into a new list
        /// </summary>
        private PickupAndPlaceMove(PickupAndPlaceMove source)
            : base(source.Moves)
        {
        }

        /// <summary>
        /// Start building the PickupAndPlaceMove by starting with just the PickUpMove
        /// </summary>
        public PickupAndPlaceMove(PickUpMove pickupMove)
        {
            base.AddToChain(pickupMove);
        }

        /// <summary>
        /// Gets the PickUpMove part of this chain of moves
        /// </summary>
        public PickUpMove PickUpMove { get { return (PickUpMove)base[0]; } }

        /// <summary>
        /// Create a shallow copy of this move
        /// </summary>
        /// <returns></returns>
        public PickupAndPlaceMove ShallowCopy()
        {
            return new PickupAndPlaceMove(this);
        }

        public override string Notate()
        {
            var sb = new System.Text.StringBuilder();
            var pickup = this.PickUpMove;
            if (pickup.PickUpCount > 1 || pickup.Remaining != 0)
                sb.Append(pickup.PickUpCount);
            sb.Append(pickup.Position.Describe());
            var delta = ((PlacePieceMove)Moves[1]).Pos - pickup.Position;
            sb.Append(Direction.DescribeDelta(delta));
            if (Moves.Count > 2)
            {
                for (int i = 1; i < Moves.Count; )
                {
                    int unstackCount = 1;
                    while (i + unstackCount < Moves.Count &&
                        ((PlacePieceMove)Moves[i]).Pos == ((PlacePieceMove)Moves[i + unstackCount]).Pos)
                        unstackCount++;
                    i += unstackCount;
                    sb.Append(unstackCount);
                }
            }
            return sb.ToString();
        }

        public IEnumerable<int> GetUnstackCounts()
        {
            for (int i = 1; i < Moves.Count; )
            {
                int unstackCount = 1;
                while (i + unstackCount < Moves.Count &&
                    ((PlacePieceMove)Moves[i]).Pos == ((PlacePieceMove)Moves[i + unstackCount]).Pos)
                    unstackCount++;
                i += unstackCount;
                yield return unstackCount;
            }
        }

        public override string ToString()
        {
            return Notate();
        }

        public int? UnstackDirection
        {
            get
            {
                if (Moves.Count < 2)
                    return null;
                else
                {
                    var delta = ((PlacePieceMove)Moves[1]).Pos - PickUpMove.Position;
                    return Direction.FromDelta(delta);
                }
            }
        }
    }
}
