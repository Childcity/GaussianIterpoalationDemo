using System;
using System.Globalization;
using System.Text;

namespace SystemSolver {

    public static class GaussJordanElimination {
        private static readonly string logFormatter = "F3";

        public static readonly bool LogIsOn = false;

        // a - matrix Ab
        // n - order of Matrix(n)
        /*
            Example b:
            double a[,] = {{ 0, 2, 1, 4 },
                        { 1, 1, 2, 6 },
                        { 2, 1, 1, 7 }};
            b = 4, 6, 7
         */
        public static double[] SolveSystem(double[,] a, int n)
        {
            // Performing Matrix transformation
            int flag = performOperation(a, n);

            if (flag == 1)
                flag = checkConsistency(a, n, flag);

            // Printing Final Matrix
            writeLine("Final Augmented Matrix is: ");
            printMatrix(a, n);

            // Return Solutions(if exist)
            return getSolution(a, n, flag);
        }

        // function to reduce matrix to reduced
        // row echelon form.
        private static int performOperation(double[,] a, int n)
        {
            int i, j, c, flag = 0;

            // Performing elementary operations
            for (i = 0; i < n; i++) {
                int k;
                if (a[i, i] == 0) {
                    c = 1;
                    while ((i + c) < n && a[i + c, i] == 0) {
                        c++;
                        writeLine("i:" + i + " c:" + c + " i+c:" + (i + c));
                    }
                    if ((i + c) == n) {
                        flag = 1;
                        break;
                    }
                    for (j = i, k = 0; k <= n; k++) {
                        double temp = a[j, k];
                        a[j, k] = a[j + c, k];
                        a[j + c, k] = temp;
                    }
                }

                for (j = 0; j < n; j++) {

                    // Excluding all i == j
                    if (i != j) {

                        // Converting Matrix to reduced row
                        // echelon form(diagonal matrix)
                        double p = a[j, i] / a[i, i];

                        for (k = 0; k <= n; k++)
                            a[j, k] = a[j, k] - (a[i, k]) * p;
                    }
                }
            }
            return flag;
        }

        // Function to get the desired result
        // if unique solutions exists, otherwise
        // prints no solution or infinite solutions
        // depending upon the input given.
        private static double[] getSolution(double[,] a, int n, int flag)
        {
            if (flag == 2) {
                throw new InfiniteSolutionException();
            } else if (flag == 3) {
                throw new NoSolutionException();
            }

            double[] solution = new double[n];
            StringBuilder line = new StringBuilder("Result is: ");

            // Getting the solution by dividing constants by
            // their respective diagonal elements
            for (int i = 0; i < n; i++) {
                solution[i] = a[i, n] / a[i, i];
                line.Append(solution[i].ToString(logFormatter)).Append(" ");
            }

            writeLine(line.ToString());

            return solution;
        }

        // To check whether infinite solutions
        // exists or no solution exists
        private static int checkConsistency(double[,] a, int n, int flag)
        {
            int i, j;
            double sum;

            // flag == 2 for infinite solution
            // flag == 3 for No solution
            flag = 3;
            for (i = 0; i < n; i++) {
                sum = 0;
                for (j = 0; j < n; j++)
                    sum += a[i, j];
                if (sum == a[i, j])
                    flag = 2;
            }
            return flag;
        }

        // Function to print the matrix
        private static void printMatrix(double[,] a, int n)
        {
            for (int i = 0; i < n; i++) {
                StringBuilder line = new StringBuilder();
                for (int j = 0; j <= n; j++)
                    line.Append(a[i, j].ToString(logFormatter)).Append(" ");
                writeLine(line.ToString());
            }
        }

        private static void writeLine(string line)
        {
            if(LogIsOn)
                writeLine(line);
        }
    }

}