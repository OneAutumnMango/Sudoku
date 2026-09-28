using System.Numerics;
using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class SimpleColouringTechnique : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Advanced;

    // this also adds the opportunity to remove all candidates that see both red and blue but i havent done that yet
    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        int applied = 0;

        // generate graphs of conjugate pairs for each candidate
        // colour each subgraph (connected component)
        // check degree 1 cells for collisions
        // remove all candidates for a colour if a collision is found

        var graphs = GenerateAllConjugatePairGraphs(puzzle);

        for (byte cand = 1; cand <= puzzle.Board.Size; cand++)
        {
            var graph = graphs[cand];

            foreach (var component in GetColouredConnectedComponents(graph))
            {
                applied += ApplySameColourCollision(
                    puzzle,
                    cand,
                    graph,
                    component.Cells,
                    component.Colours);
            }
        }

        return applied;
    }


    private Dictionary<byte, Dictionary<Cell, HashSet<Cell>>> GenerateAllConjugatePairGraphs(Puzzle puzzle)
    {
        var constraintGraphs = puzzle.RuleSet
            .GetConstraints()
            .Select(GenerateConjugatePairsAdjacencyList)
            .ToList();

        return Enumerable.Range(1, puzzle.Board.Size)
            .Select(cand => (byte)cand)
            .ToDictionary(
                cand => cand,
                cand => constraintGraphs
                    .SelectMany(graph => graph.GetValueOrDefault(cand, []))
                    .GroupBy(pair => pair.Key)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .SelectMany(pair => pair.Value)
                            .ToHashSet()
                    )
            );
    }

    private IEnumerable<ColouredComponent> GetColouredConnectedComponents(
        Dictionary<Cell, HashSet<Cell>> graph)
    {
        var visited = new HashSet<Cell>();

        // BFS for all connected components
        foreach (var startingCell in graph.Keys)
        {
            if (!visited.Add(startingCell))
                continue;

            var queue = new Queue<Cell>();
            var colours = new Dictionary<Cell, Colour>();
            var componentCells = new HashSet<Cell>();

            colours[startingCell] = Colour.Red;
            componentCells.Add(startingCell);
            queue.Enqueue(startingCell);

            // BFS
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                var nextColour = colours[current] == Colour.Red ? Colour.Blue : Colour.Red;

                foreach (var neighbour in graph[current])
                {
                    // colouring conflict cannot occur (at least it shouldnt i think) but this is a safety check anyways
                    if (colours.TryGetValue(neighbour, out var existingColour))
                    {
                        if (existingColour != nextColour)
                        {
                            throw new InvalidOperationException(
                                "Conjugate-pair colouring conflict.");
                        }

                        continue;
                    }

                    colours[neighbour] = nextColour;
                    componentCells.Add(neighbour);
                    visited.Add(neighbour);
                    queue.Enqueue(neighbour);
                }
            }

            yield return new ColouredComponent(componentCells, colours);
        }
    }

    private int ApplySameColourCollision(
        Puzzle puzzle,
        byte cand,
        Dictionary<Cell, HashSet<Cell>> graph,
        HashSet<Cell> componentCells,
        Dictionary<Cell, Colour> colours)
    {
        int applied = 0;

        var degreeOneCells = componentCells
            .Where(cell => graph[cell].Count == 1)
            .ToList();

        var degreeOneCellsByConstraint = new Dictionary<IConstraint, HashSet<Cell>>();

        foreach (var cell in degreeOneCells)
        {
            foreach (var constraint in puzzle.RuleSet.GetContainingConstraints(cell))
            {
                if (!degreeOneCellsByConstraint.TryGetValue(constraint, out var cells))
                {
                    cells = [];
                    degreeOneCellsByConstraint[constraint] = cells;
                }

                cells.Add(cell);
            }
        }

        foreach (var cells in degreeOneCellsByConstraint.Values)
        {
            // if collision remove all cells' candidates from colours with that colours
            foreach (var group in cells.GroupBy(cell => colours[cell]))
            {
                if (group.Count() < 2)
                    continue;

                foreach (var cell in componentCells)
                {
                    if (colours[cell] != group.Key)
                        continue;

                    if (!cell.HasCandidate(cand))
                        continue;

                    puzzle.RemoveCandidate(cell, cand);
                    applied++;
                }
            }
        }

        return applied;
    }

    private Dictionary<byte, Dictionary<Cell, HashSet<Cell>>> GenerateConjugatePairsAdjacencyList(IConstraint constraint)
    {
        var result = new Dictionary<byte, Dictionary<Cell, HashSet<Cell>>>();

        var cellCount = constraint.Cells.Count;

        Cell[] first = new Cell[cellCount + 1];
        Cell[] second = new Cell[cellCount + 1];
        byte[] counts = new byte[cellCount + 1];

        foreach (var cell in constraint.Cells)
        {
            if (cell.Value != 0)
                continue;

            ushort candidates = cell.Candidates;

            while (candidates != 0)
            {
                int cand = BitOperations.TrailingZeroCount(candidates) + 1;
                candidates &= (ushort)(candidates - 1);  // pop smallest bit

                if (counts[cand] == 0)
                    first[cand] = cell;
                else if (counts[cand] == 1)
                    second[cand] = cell;

                counts[cand]++;
            }
        }

        for (byte cand = 1; cand <= cellCount; cand++)
        {
            if (counts[cand] != 2)
                continue;

            var cell1 = first[cand];
            var cell2 = second[cand];

            if (!result.TryGetValue(cand, out var adjacency))
            {
                adjacency = [];
                result[cand] = adjacency;
            }

            adjacency.TryAdd(cell1, []);
            adjacency.TryAdd(cell2, []);

            adjacency[cell1].Add(cell2);
            adjacency[cell2].Add(cell1);
        }

        return result;
    }



    private enum Colour
    {
        Red,
        Blue
    }

    private sealed record ColouredComponent(
        HashSet<Cell> Cells,
        Dictionary<Cell, Colour> Colours);
}