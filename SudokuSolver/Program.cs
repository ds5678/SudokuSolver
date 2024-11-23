using Google.OrTools.ConstraintSolver;

namespace SudokuSolver;

internal class Program
{
	static void Main(string[] args)
	{
		byte[] data = new byte[81];

		// Instantiate the solver.
		Solver solver = new Solver("Sudoku");

		// Create the variables.
		IntVar[,] variables = new IntVar[9, 9];
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				byte value = data[i * 9 + j];
				variables[i, j] = value is > 0 and <= 9
					? solver.MakeIntConst(value)
					: solver.MakeIntVar(1, 9, $"cell_{i}_{j}");
			}
		}

		// Add constraints.
		for (int i = 0; i < 9; i++)
		{
			AddMakeAllDifferent(solver, GetRow(variables, i));
			AddMakeAllDifferent(solver, GetColumn(variables, i));
			AddMakeAllDifferent(solver, GetSquare(variables, i));
		}
		Console.WriteLine($"Number of constraints: {solver.Constraints()}");

		// Solve the problem.
		DecisionBuilder db =
			solver.MakePhase(new IntVarVector(GetAll(variables)), Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);

		// Print solution on console.
		solver.NewSearch(db);
		if (solver.NextSolution())
		{
			foreach (IntVar variable in GetAll(variables))
			{
				Console.WriteLine($"{variable.Name()} : {variable.Value()} ");
			}
		}
		solver.EndSearch();
		Console.WriteLine($"Number of solutions found before ending the search: {solver.Solutions()}");

		Console.WriteLine("Advanced usage:");
		Console.WriteLine($"Problem solved in {solver.WallTime()}ms");
		Console.WriteLine($"Memory usage: {Solver.MemoryUsage()}bytes");
	}

	private static void AddMakeAllDifferent(Solver solver, IEnumerable<IntVar> variables)
	{
		solver.Add(solver.MakeAllDifferent(new IntVarVector(variables)));
	}

	private static IEnumerable<IntVar> GetRow(IntVar[,] variables, int row)
	{
		for (int i = 0; i < 9; i++)
		{
			yield return variables[row, i];
		}
	}

	private static IEnumerable<IntVar> GetColumn(IntVar[,] variables, int column)
	{
		for (int i = 0; i < 9; i++)
		{
			yield return variables[i, column];
		}
	}

	private static IEnumerable<IntVar> GetSquare(IntVar[,] variables, int square)
	{
		int row = square / 3;
		int column = square % 3;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				yield return variables[row * 3 + i, column * 3 + j];
			}
		}
	}

	private static IEnumerable<IntVar> GetAll(IntVar[,] variables)
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				yield return variables[i, j];
			}
		}
	}
}
