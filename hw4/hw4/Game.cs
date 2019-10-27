using System;
using System.IO;
using System.Runtime.Serialization;

namespace Hw4
{
    public enum CellState
    {
        Nothing,
        Empty,
        Wall,
        Exit
    }

    public enum Movement
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class MovementUtils
    {
        public static (int dy, int dx) Offset(this Movement movement)
        {
            switch (movement)
            {
                case Movement.Up:
                    return (-1, 0);
                case Movement.Down:
                    return (+1, 0);
                case Movement.Left:
                    return (0, -1);
                case Movement.Right:
                    return (0, +1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(movement), movement, null);
            }
        }
    }

    [Serializable]
    public class GameFileFormatException : Exception
    {
        internal GameFileFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GameFileFormatException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    public class Game
    {
        public Game(string fileName)
        {
            try
            {
                using (var file = File.OpenText(fileName))
                {
                    var sizes = file.ReadLine().Split(' ');
                    if (sizes.Length != 2)
                        throw new FormatException("Wrong sizes line");
                    var height = int.Parse(sizes[0]);
                    var width = int.Parse(sizes[1]);

                    var characterCoordinates = file.ReadLine().Split(' ');
                    if (characterCoordinates.Length != 2)
                        throw new FormatException("Wrong character coordinates line");
                    CharacterPosition = (int.Parse(characterCoordinates[0]), int.Parse(characterCoordinates[1]));

                    _board = new CellState[height, width];
                    for (var i = 0; i < height; ++i)
                    {
                        var line = file.ReadLine();
                        if (line.Length != width)
                            throw new FormatException("Line has wrong length");
                        for (var j = 0; j < width; ++j)
                            switch (line[j])
                            {
                                case ' ':
                                    _board[i, j] = CellState.Empty;
                                    break;
                                case '#':
                                    _board[i, j] = CellState.Wall;
                                    break;
                                case 'E':
                                    _board[i, j] = CellState.Exit;
                                    break;
                                default:
                                    throw new FormatException($"Unknown char {line[j]}");
                            }
                    }

                    if (this[CharacterPosition] != CellState.Empty)
                        throw new FormatException("Character must be placed in empty cell");
                }
            }
            catch (IOException e)
            {
                throw new GameFileFormatException($"Failed to read the level from {fileName}", e);
            }
            catch (FormatException e)
            {
                throw new GameFileFormatException($"Failed to read the level from {fileName}", e);
            }
        }

        public void Move(Movement movement)
        {
            var offset = movement.Offset();
            (int y, int x) newPosition = (CharacterPosition.y + offset.dy, CharacterPosition.x + offset.dx);
            newPosition.y = Math.Max(0, Math.Min(Height - 1, newPosition.y));
            newPosition.x = Math.Max(0, Math.Min(Width - 1, newPosition.x));
            if (this[newPosition] != CellState.Wall)
                CharacterPosition = newPosition;
        }

        public bool IsOver => this[CharacterPosition] == CellState.Exit;

        public int Height => _board.GetLength(0);
        public int Width => _board.GetLength(1);
        public (int y, int x) CharacterPosition { get; private set; }

        public CellState this[int y, int x] => _board[y, x];
        public CellState this[(int y, int x) coords] => this[coords.y, coords.x];

        private readonly CellState[,] _board;
    }
}
