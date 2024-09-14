using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2023_day_10;

public class PipeMap
{
    private char[,] _map;
    private int _height => _map.GetLength(0);
    private int _width => _map.GetLength(1);

    private Coordinate _startPosition;
    private List<Coordinate> _solutionPath;
    private char this[Coordinate c] => _map[c.Row, c.Col];

    public PipeMap(IList<string> map)
    {
        // first dimension is row (incrementing downwards)
        // second dimension is col (incrementing rightwards)

        _map = new char[map.Count, map[0].Length];

        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                _map[i, j] = map[i][j];
                if (map[i][j] == 'S')
                    _startPosition = new(i, j);
            }
        }

        _solutionPath = SolvePath();
        Console.WriteLine($"Solution path contains {_solutionPath.Count} cells.  Half of that is: {_solutionPath.Count / 2}");
    }

    private List<Coordinate> SolvePath()
    {
        List<Coordinate> result = [_startPosition];
        HashSet<Coordinate> visited = [];

        (Coordinate nextCell, Direction curDirection) = GetInitialMove();

        int infiniteDetector = 0;
        while (nextCell != _startPosition)
        {
            infiniteDetector++;
            if (infiniteDetector >= 1000000)
                break;

            result.Add(nextCell);
            visited.Add(nextCell);

            var prevCell = nextCell;
            curDirection = GetNextDirection(curDirection, this[prevCell]);
            nextCell = GetNextCellFromDirection(nextCell, curDirection);
        }

        return result;
    }

    private (Coordinate, Direction) GetInitialMove()
    {
        if (_startPosition.Row == 0)
        {
            if (_startPosition.Col == 0 || _startPosition.Col == _width - 1)
            {
                // we're in one of the top corners.  Only 2 connected cells. 
                return (
                    new(_startPosition.Row + 1, _startPosition.Col),
                    Direction.Down
                    );
            }

            // we're on the top edge, not a corner. 
            // the cell to the left is connected if (and only if) it's - or F
            // if it's connected, we choose it.  Otherwise it _must_ be the other two
            char leftCell = _map[_startPosition.Row, _startPosition.Col - 1];
            if (leftCell == '-' || leftCell == 'F')
            {
                return (
                    new(_startPosition.Row, _startPosition.Col - 1),
                    Direction.Left
                    );
            }
            return (
                new(_startPosition.Row, _startPosition.Col + 1),
                Direction.Right
                );
        }

        if (_startPosition.Row == _height - 1)
        {
            // we're in the bottom row
            if (_startPosition.Col == 0 || _startPosition.Col == _width - 1)
            {
                // we're in one of the bottom corners.  Only 2 connected cells. 
                return (
                    new(_startPosition.Row - 1, _startPosition.Col),
                    Direction.Up
                    );
            }

            // we're on the bottom edge.  
            char leftCell = _map[_startPosition.Row, _startPosition.Col - 1];
            if (leftCell == '-' || leftCell == 'L')
            {
                return (
                    new(_startPosition.Row, _startPosition.Col - 1),
                    Direction.Left
                    );
            }
            return (
                new(_startPosition.Row, _startPosition.Col + 1),
                Direction.Right
                );

        }

        if (_startPosition.Col == 0)
        {
            // we're on left edge, not a corner
            char aboveCell = _map[_startPosition.Row - 1, _startPosition.Col];
            if (aboveCell == '|' || aboveCell == 'F')
            {
                return (
                    new(_startPosition.Row - 1, _startPosition.Col),
                    Direction.Up
                    );
            }
            return (
                new(_startPosition.Row + 1, _startPosition.Col),
                Direction.Down
                );
        }

        if (_startPosition.Col == _width - 1)
        {
            // we're on right edge, not a corner
            char aboveCell = _map[_startPosition.Row - 1, _startPosition.Col];
            if (aboveCell == '|' || aboveCell == '7')
            {
                return (
                    new(_startPosition.Row - 1, _startPosition.Col),
                    Direction.Up
                    );
            }
            return (
                new(_startPosition.Row + 1, _startPosition.Col),
                Direction.Down
                );
        }

        // we're in the middle somewhere. 
        char cellAbove = _map[_startPosition.Row - 1, _startPosition.Col];
        if (cellAbove == '|' || cellAbove == 'F' || cellAbove == '7')
            return (
                new(_startPosition.Row - 1, _startPosition.Col),
                Direction.Up
                );

        char cellBelow = _map[_startPosition.Row + 1, _startPosition.Col];
        if (cellBelow == '|' || cellBelow == 'J' || cellBelow == 'L')
            return (
                new(_startPosition.Row + 1, _startPosition.Col),
                Direction.Down
                );

        // if we're not connected above or below, we _must_ be connected left-right
        return (
            new(_startPosition.Row, _startPosition.Col + 1),
            Direction.Right
            );
    }

    private Direction GetNextDirection(Direction entryDirection, char pipe)
    {
        switch (pipe)
        {
            case '|':
            case '-':
                return entryDirection;

            case 'J':
                if (entryDirection == Direction.Right)
                    return Direction.Up;
                return Direction.Left;

            case 'F':
                if (entryDirection == Direction.Up)
                    return Direction.Right;
                return Direction.Down;

            case 'L':
                if (entryDirection == Direction.Down)
                    return Direction.Right;
                return Direction.Up;

            case '7':
                if (entryDirection == Direction.Right)
                    return Direction.Down;
                return Direction.Left;

            default: throw new InvalidOperationException();
        }
    }

    private Coordinate GetNextCellFromDirection(Coordinate current, Direction direction)
    {
        return direction switch
        {
            Direction.Up => new(current.Row - 1, current.Col),
            Direction.Down => new(current.Row + 1, current.Col),
            Direction.Left => new(current.Row, current.Col - 1),
            Direction.Right => new(current.Row, current.Col + 1),
            _ => throw new InvalidOperationException()
        };
    }
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}


public readonly struct Coordinate
{
    public int Row { get; init; }
    public int Col { get; init; }

    public Coordinate(int row, int col) { Row = row; Col = col; }
    public Coordinate() { }

    public static bool operator != (Coordinate left, Coordinate right)
    {
        return (left.Row != right.Row) || (left.Col != right.Col);
    }

    public static bool operator == (Coordinate left, Coordinate right)
    {
        return (left.Row == right.Row) && (left.Col == right.Col);
    }

    public override string ToString()
    {
        return $"({Row},{Col})";
    }
}
