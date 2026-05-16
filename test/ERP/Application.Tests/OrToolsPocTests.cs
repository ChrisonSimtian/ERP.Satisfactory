using Google.OrTools.LinearSolver;

namespace ERP.Application.Tests;

/// <summary>
/// Throwaway PoC (#88) — confirms Google.OrTools loads its native runtime on
/// the dev + CI hosts and can solve a trivial LP. Delete once the real LP
/// planner adapter lands.
/// </summary>
public sealed class OrToolsPocTests
{
    [Fact]
    public void Glop_solves_trivial_LP()
    {
        var solver = Solver.CreateSolver("GLOP");
        Assert.NotNull(solver);

        var x = solver.MakeNumVar(0.0, double.PositiveInfinity, "x");
        var y = solver.MakeNumVar(0.0, double.PositiveInfinity, "y");

        solver.Add(x <= 3);
        solver.Add(y <= 2);

        var objective = solver.Objective();
        objective.SetCoefficient(x, 1);
        objective.SetCoefficient(y, 1);
        objective.SetMaximization();

        var status = solver.Solve();

        Assert.Equal(Solver.ResultStatus.OPTIMAL, status);
        Assert.Equal(3.0, x.SolutionValue(), 6);
        Assert.Equal(2.0, y.SolutionValue(), 6);
        Assert.Equal(5.0, objective.Value(), 6);
    }
}
