using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;
using Sudoku.Core.Solver.Graphs;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class SimpleColouringTechnique : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Advanced;

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        int applied = 0;

        // generate graphs of conjugate pairs for each candidate
        // colour each subgraph (connected component)
        // check degree 1 cells for collisions
        // remove all candidates for a colour if a collision is found

        var graphs = CandidateChainGraphGenerator.GenerateStrongGraph(puzzle);

        for (byte cand = 1; cand <= puzzle.Board.Size; cand++)
        {
            var graph = graphs[cand];

            foreach (var component in GetColouredConnectedComponents(graph))
            {
                applied += ApplyOppositeColourElimination(
                    puzzle,
                    cand,
                    graph,
                    component.Cells,
                    component.Colours);
                
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


    private IEnumerable<ColouredComponent> GetColouredConnectedComponents(
        CandidateChainGraph graph)
    {
        var visited = new HashSet<Cell>();

        // BFS for all connected components
        foreach (var startingCell in graph.Vertices)
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

                foreach (var neighbour in graph.GetEdges(current).Keys)
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
        CandidateChainGraph graph,
        HashSet<Cell> componentCells,
        Dictionary<Cell, Colour> colours)
    {
        int applied = 0;

        var degreeOneCells = componentCells
            .Where(cell => graph.GetEdges(cell).Count == 1)
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

    private int ApplyOppositeColourElimination(
        Puzzle puzzle,
        byte cand,
        CandidateChainGraph graph,
        HashSet<Cell> componentCells,
        Dictionary<Cell, Colour> colours)
    {
        int applied = 0;

        // union of cells in constraints with a red, and constraints with a blue
        // intersect both sets
        // remove candidates that arent coloured

        var redCells = colours
            .Where(pair => pair.Value == Colour.Red)
            .Select(pair => pair.Key);

        var blueCells = colours
            .Where(pair => pair.Value == Colour.Blue)
            .Select(pair => pair.Key);

        var cellsSeeingRed = redCells
            .SelectMany(cell => puzzle.RuleSet.GetContainingConstraints(cell))
            .SelectMany(constraint => constraint.Cells)
            .ToHashSet();

        var cellsSeeingBlue = blueCells
            .SelectMany(cell => puzzle.RuleSet.GetContainingConstraints(cell))
            .SelectMany(constraint => constraint.Cells)
            .ToHashSet();

        var cellsSeeingBothColours = cellsSeeingRed
            .Intersect(cellsSeeingBlue)
            .Where(cell => !colours.ContainsKey(cell)
                && cell.Value == 0
                && cell.HasCandidate(cand));

        foreach (var cell in cellsSeeingBothColours)
        {
            puzzle.RemoveCandidate(cell, cand);
            applied++;
        }

        return applied;
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