#define MULTITHREADED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakEngine
{
    public class TakAI2 : ITakAI
    {
        /// <summary>
        /// Default value for the maximum game tree search depth
        /// </summary>
        const int DefaultMaxDepth = 3;
        BoardPosition[] _randomPositions;
        BoardPosition[] _normalPositions;

        /* Notes on maximum depth:
         * Higher values increase the strength of the AI but also make it much slower.
         * In the late game there can be more than 100 moves in a single board position, so looking 1 extra move into the
         * future can be 100x slower.  That is the worst case scenario, though, which is often mitigated by alpha-beta
         * pruning.  So in practice the AI does not get slower _quite_ that fast... usually... but it varies by board position and
         * some positions don't get much benefit from pruning.  There's also a bit of luck involved based on the order in which
         * the AI considers moves.
         */

        /// <summary>
        /// Maximum game tree search depth.
        /// </summary>
        int _maxDepth;

        /// <summary>
        /// Psuedo-random number generator is only used to randomize the order in which positions are considered.
        /// </summary>
        Random _rand;
        AIThreadData _singleThreadData;
        bool _canceled = false;
        bool _canceling = false;

        long _evals = 0;
        System.Diagnostics.Stopwatch _timer = new System.Diagnostics.Stopwatch();
        static string DebugLogFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tak_ai_debug.txt");

        public BoardPosition[] RandomPositions { get { return _randomPositions; } }
        public BoardPosition[] NormalPositions { get { return _normalPositions; } }
        public int LastEvaluation { get; private set; }
        public string EvalMethod { get { return "InfEval"; } }
        public TakAI2(int boardSize, int maxDepth = DefaultMaxDepth)
        {
            _maxDepth = maxDepth;
            _rand = new Random();
            _singleThreadData = new AIThreadData(boardSize);

            // Initialize list of all legal board positions
            _normalPositions = new BoardPosition[boardSize * boardSize];
            for (int i = 0; i < _normalPositions.Length; i++)
                _normalPositions[i] = new BoardPosition(i % boardSize, i / boardSize);

            // Copy board positions into an alternate array that can be randomized to make the AI more interesting
            _randomPositions = new BoardPosition[boardSize * boardSize];
            Array.Copy(_normalPositions, _randomPositions, _normalPositions.Length);
        }

        public void Cancel()
        {
            _canceling = true;
        }
        public bool Canceled { get { return _canceled; } }

        public IMove FindGoodMove(GameState game)
        {
            // Clear cancelation state before beginning to process the move
            _canceled = _canceling = false;

            _evals = 0;
            _timer.Stop();
            _timer.Reset();
            _timer.Restart();

            // Randomize order of board positions so that the AI appears less predictable
            // in the early phases of the game
            _randomPositions.RandomizeOrder(_rand);

            IMove bestMove;
            int bestEval;

#if MULTITHREADED
            FindGoodMoveMulti(game, 0, out bestMove, out bestEval);
            LastEvaluation = bestEval;
#else
            _singleThreadData.Game = game;
            FindGoodMove(_singleThreadData, 0, null, out bestMove, out bestEval);
            LastEvaluation = bestEval;
#endif

            _timer.Stop();
            if (Properties.Settings.Default.debug)
            {
                System.IO.File.AppendAllText(DebugLogFilePath,
                    string.Format("{0}\t{1}\t{2:0.0000}\t{3:0.0000}\r\n",
                    EvalMethod,
                    _evals,
                    _timer.Elapsed.TotalSeconds,
                    (double)_evals / _timer.Elapsed.TotalSeconds));
            }

            return bestMove;
        }

        /// <summary>
        /// Gets or sets the AI's search tree depth (i.e. difficulty).
        /// NOTE: Depth N+1 is roughly 30 to 50 times slower than depth N.
        /// NOTE: The AI actually gets slower in the late game due to bigger stacks of stones
        /// </summary>
        public int MaxDepth { get { return _maxDepth; } set { _maxDepth = value; } }

        void FindGoodMove(AIThreadData data, int depth, int? prune, out IMove bestMove, out int bestEval)
        {
            bestMove = null;
            bestEval = int.MinValue;
            if (_canceling)
            {
                _canceled = true;
                return;
            }
            var depthMoves = data.DepthMoves;
            while (depth >= depthMoves.Count)
                depthMoves.Add(new List<IMove>());
            var moves = depthMoves[depth];
            moves.Clear();

            var game = data.Game;
            EnumerateMoves(moves, game, _randomPositions);

            foreach (var move in moves)
            {
                move.MakeMove(game);
                game.Ply++;

                int eval;
                bool gameOver;
                data.Evaluator.Evaluate(game, out eval, out gameOver);
                _evals++;
                if (0 == (game.Ply & 1))
                    eval *= -1;
                if (!(depth == _maxDepth || gameOver))
                {
                    IMove opmove;
                    int opeval;
                    FindGoodMove(data, depth + 1, bestMove == null ? (int?)null : -bestEval, out opmove, out opeval);
                    eval = opeval * -1;
                }
                if (eval > bestEval)
                {
                    bestMove = move;
                    bestEval = eval;
                }

                game.Ply--;
                move.TakeBackMove(game);

                if (gameOver && eval > 0)
                    break;
                if (prune.HasValue && eval >= prune.Value)
                    break;
            }
        }

        class AIThreadData
        {
            public Evaluator Evaluator;
            public GameState Game;
            public List<List<IMove>> DepthMoves;
            public long Evals = 0;
            public AIThreadData(int size, GameState game = null)
            {
                Evaluator = new Evaluator(size);
                if (game != null)
                    Game = game.DeepCopy();
                DepthMoves = new List<List<IMove>>();
            }
        }

        class IterThreadData : AIThreadData
        {
            public IMove BestMove = null;
            public int BestEval = int.MinValue;
            public IterThreadData(GameState game)
                : base(game.Size, game: game)
            {
            }
        }

        /// <summary>
        /// Multithreaded version of the FindGoodMove 
        /// </summary>
        void FindGoodMoveMulti(GameState game, int depth, out IMove bestMove, out int bestEval)
        {
            if (depth != 0)
                throw new ApplicationException();
            var depthMoves = _singleThreadData.DepthMoves;
            while (depth >= _singleThreadData.DepthMoves.Count)
                depthMoves.Add(new List<IMove>());
            var moves = depthMoves[depth];
            moves.Clear();

            EnumerateMoves(moves, game, _randomPositions);

            IMove sharedBestMove = null;
            int sharedBestEval = int.MinValue;
            object sync = new object();
            Parallel.ForEach<IMove, IterThreadData>(moves,
                () =>
                {
                    // Initialize a thread's local data
                    var data = new IterThreadData(game);
                    lock (sync)
                    {
                        data.BestMove = sharedBestMove;
                        data.BestEval = sharedBestEval;
                    }
                    return data;
                },
                (move, loopState, data) =>
                {
                    // synchronize thread's best move
                    lock (sync)
                    {
                        if (sharedBestEval > data.BestEval)
                        {
                            data.BestEval = sharedBestEval;
                            data.BestMove = sharedBestMove;
                        }
                    }

                    // make the move
                    move.MakeMove(data.Game);
                    data.Game.Ply++;

                    int eval;
                    bool gameOver;
                    data.Evaluator.Evaluate(data.Game, out eval, out gameOver);
                    data.Evals++;
                    if (0 == (data.Game.Ply & 1))
                        eval *= -1;
                    if (!(depth == _maxDepth || gameOver))
                    {
                        IMove opmove;
                        int opeval;
                        FindGoodMove(data, depth + 1, data.BestMove == null ? (int?)null : -data.BestEval, out opmove, out opeval);
                        eval = opeval * -1;
                    }
                    if (eval > data.BestEval)
                    {
                        data.BestEval = eval;
                        data.BestMove = move;
                    }

                    data.Game.Ply--;
                    move.TakeBackMove(data.Game);

                    // synchronize shared best move
                    lock (sync)
                    {
                        if (data.BestEval > sharedBestEval)
                        {
                            sharedBestEval = data.BestEval;
                            sharedBestMove = data.BestMove;
                        }
                    }

                    if (gameOver && eval > 0)
                        loopState.Stop();
                    return data;
                },
                data =>
                {
                    _evals += data.Evals;
                    // do nothing
                });

            bestMove = sharedBestMove;
            bestEval = sharedBestEval;
        }

        /// <summary>
        /// Enumerate all legal moves in the current board position
        /// </summary>
        /// <param name="dest">Destination list into which moves will be added</param>
        /// <param name="game">Current game state</param>
        /// <param name="moveOrder">Order in which board positions will be considered</param>
        public static void EnumerateMoves(IList<IMove> dest, GameState game, IList<BoardPosition> moveOrder)
        {
            int player = game.Ply & 1;
            if (game.Ply < 2)
            {
                // place enemy flat stone on empty squares
                player = player ^ 1;
                foreach (var pos in moveOrder)
                {
                    if (!game[pos].HasValue)
                        dest.Add(new PlacePieceMove(Piece.MakePieceID(Piece.Stone_Flat, player), pos, true, false));
                }
            }
            else
            {
                // place stones on empty squares
                var sremain = game.StonesRemaining[player];
                var cremain = game.CapRemaining[player];
                foreach (var pos in moveOrder)
                {
                    if (!game[pos].HasValue)
                    {
                        if (sremain > 0)
                        {
                            dest.Add(new PlacePieceMove(Piece.MakePieceID(Piece.Stone_Flat, player), pos, true, false));
                            dest.Add(new PlacePieceMove(Piece.MakePieceID(Piece.Stone_Standing, player), pos, true, false));
                        }
                        if (cremain > 0)
                            dest.Add(new PlacePieceMove(Piece.MakePieceID(Piece.Stone_Cap, player), pos, true, false));
                    }
                }

                // Move stacks
                foreach (var pos in moveOrder)
                {
                    var stack = game.Board[pos.X, pos.Y];
                    if (stack.Count == 0)
                        continue;
                    var topPiece = stack[stack.Count - 1];
                    var topStone = Piece.GetStone(topPiece);
                    if (Piece.GetPlayerID(topPiece) != player)
                        continue;

                    for (int pickupCount = 1; pickupCount <= Math.Min(stack.Count, game.Size); pickupCount++)
                    {
                        var pickupMove = new PickUpMove(pos, pickupCount, game);
                        var template = new PickupAndPlaceMove(pickupMove);
                        for (int dir = 0; dir < 4; dir++)
                            EnumUnstackMoves(dest, game, template, 0, pos, dir);
                    }
                }
            }
        }

        static void EnumUnstackMoves(IList<IMove> dest, GameState state,
            PickupAndPlaceMove template, int start, BoardPosition pos, int dir)
        {
            var targetPos = Direction.Offset(pos, dir);
            if (!state.IsPositionLegal(targetPos))
                return;
            var stack = state.Board[targetPos.X, targetPos.Y];

            var stacktop = state[targetPos];
            bool flatten = false;
            if (stacktop.HasValue)
            {
                var topstone = Piece.GetStone(stacktop.Value);

                // destination stack must not have cap stone
                if (topstone == Piece.Stone_Cap)
                    return;

                // only capstone by itself may flatten standing stone
                if (topstone == Piece.Stone_Standing)
                {
                    if (template.PickUpMove.PickUpCount - start != 1)
                        return;
                    if (!template.PickUpMove.IsCapStone(start))
                        return;
                    flatten = true;
                }
            }
            var pickupMove = template.PickUpMove;
            int maxPlace = pickupMove.PickUpCount - start;
            for (int i = 1; i <= maxPlace; i++)
            {
                for (int j = 0; j < i; j++)
                    template.AddToChain(pickupMove.GeneratePlaceFromStack(j + start, targetPos, flatten));
                if (i + start == pickupMove.PickUpCount)
                    dest.Add(template.ShallowCopy());
                else
                    EnumUnstackMoves(dest, state, template, start + i, targetPos, dir);
                template.RemoveFromEnd(i);
            }
        }

        /// <summary>
        /// Provides AI's evaluation function to determine who it thinks is winning
        /// </summary>
        public class Evaluator
        {
            public const int FlatWinEval = 9000;
            FloodFill _flood = new FloodFill();
            int[,] _ids;

            int _fillingid = 0;
            int _fillContextPlayer;
            GameState _fillContextState;
            HashSet<int> _lookup = new HashSet<int>();
            int[] _roadScores = new int[2];
            int _boardSize;
            List<BoardPosition> _edgePositions;
            int[,] _inf;

            public Evaluator(int boardSize)
            {
                _ids = new int[boardSize, boardSize];
                _inf = new int[boardSize, boardSize];
                _boardSize = boardSize;

                // Precalculate list of edge positions to reduce logic that must be executed in every evaluation
                _edgePositions = new List<BoardPosition>();
                for (int i = 0; i < _boardSize; i++)
                {
                    _edgePositions.Add(new BoardPosition(i, 0));
                    _edgePositions.Add(new BoardPosition(i, _boardSize - 1));
                    if (i > 0 && i < _boardSize - 1)
                    {
                        _edgePositions.Add(new BoardPosition(0, i));
                        _edgePositions.Add(new BoardPosition(_boardSize - 1, i));
                    }
                }
            }

            /// <summary>
            /// Evaluate the current board position
            /// </summary>
            /// <param name="game">Current game state</param>
            /// <param name="eval">Evaluation function output.  Positive values favor the first player while negative values favor the second player.</param>
            /// <param name="gameOver">Indicates if any game ending condition has been reached</param>
            public void Evaluate(GameState game, out int eval, out bool gameOver)
            {
                gameOver = false;
                _fillContextState = game;

                // reset island id numbers
                for (int i = 0; i < game.Size; i++)
                    for (int j = 0; j < game.Size; j++)
                    {
                        _ids[i, j] = 0;
                        _inf[i, j] = 0;
                    }

                // Ensure every flat stone / cap stone on the edges has been assigned an island ID number
                foreach (var pos in _edgePositions)
                {
                    var piece = game[pos.X, pos.Y];
                    if (piece.HasValue && _ids[pos.X, pos.Y] == 0 && Piece.GetStone(piece.Value) != Piece.Stone_Standing)
                    {
                        _fillingid++;
                        _fillContextPlayer = Piece.GetPlayerID(piece.Value);
                        _flood.Fill(pos.X, pos.Y, doesMatch, paint);
                    }
                }

                // look for north-south road
                _lookup.Clear();
                _roadScores[0] = _roadScores[1] = 0;
                for (int x = 0; x < game.Size; x++)
                {
                    if (_ids[x, 0] > 0)
                        _lookup.Add(_ids[x, 0]);
                }
                for (int x = 0; x < game.Size; x++)
                {
                    if (_lookup.Contains(_ids[x, game.Size - 1]))
                    {
                        var player = Piece.GetPlayerID(game[x, game.Size - 1].Value);
                        _roadScores[player] = 9999 - Math.Min(500, game.Ply);
                    }
                }

                // look for east-west road
                _lookup.Clear();
                for (int y = 0; y < game.Size; y++)
                {
                    if (_ids[0, y] > 0)
                        _lookup.Add(_ids[0, y]);
                }
                for (int y = 0; y < game.Size; y++)
                {
                    if (_lookup.Contains(_ids[game.Size - 1, y]))
                    {
                        var player = Piece.GetPlayerID(game[game.Size - 1, y].Value);
                        _roadScores[player] = 9999 - Math.Min(500, game.Ply);
                    }
                }

                int score = _roadScores[0] - _roadScores[1];
                if (score != 0)
                {
                    eval = score;
                    gameOver = true;
                    return;
                }
                eval = 0;
                bool foundEmpty = false;
                for (int y = 0; y < game.Size; y++)
                    for (int x = 0; x < game.Size; x++)
                    {
                        var stack = game.Board[x, y];
                        for (int j = 0; j < stack.Count; j++)
                        {
                            var piece = stack[j];
                            int pts = 0;
                            if (Piece.GetStone(piece) == Piece.Stone_Flat)
                                pts = 2;
                            var player = Piece.GetPlayerID(piece);
                            if (player != 0)
                                pts *= -1;
                            if (j == stack.Count - 1)
                            {
                                score += pts;
                                pts *= 10;

                                int infadd = 1 - player * 2;
                                if (x > 0)
                                    _inf[x - 1, y] += infadd;
                                if (y > 0)
                                    _inf[x, y - 1] += infadd;
                                if (x < game.Size - 1)
                                    _inf[x + 1, y] += infadd;
                                if (y < game.Size - 1)
                                    _inf[x, y + 1] += infadd;
                            }
                            else if (player == Piece.GetPlayerID(stack[stack.Count - 1]))
                                pts *= 2;
                            eval += pts;
                        }
                        if (stack.Count == 0)
                            foundEmpty = true;
                    }

                for (int y = 0; y < game.Size; y++)
                    for (int x = 0; x < game.Size; x++)
                    {
                        var stack = game.Board[x, y];
                        if (stack.Count == 0)
                            eval += _inf[x, y];
                    }

                if (!foundEmpty ||
                    (game.StonesRemaining[0] + game.CapRemaining[0]) == 0 ||
                    (game.StonesRemaining[1] + game.CapRemaining[1]) == 0)
                {
                    gameOver = true;
                    if (score > 0)
                        eval = FlatWinEval;
                    else if (score < 0)
                        eval = -FlatWinEval;
                    else
                        eval = 0;
                }
            }

            bool doesMatch(int x, int y)
            {
                if (x < 0 || y < 0 || x >= _boardSize || y >= _boardSize)
                    return false;
                if (_ids[x, y] != 0)
                    return false;
                var piece = _fillContextState[x, y];
                if (!piece.HasValue)
                    return false;
                if (Piece.GetPlayerID(piece.Value) != _fillContextPlayer)
                    return false;
                if (Piece.GetStone(piece.Value) == Piece.Stone_Standing)
                    return false;
                return true;
            }

            void paint(int x, int y)
            {
                _ids[x, y] = _fillingid;
            }
        }
    }
}
