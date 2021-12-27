using System;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    public delegate int Lookup(string variable);

    // Author: Jordy A. Larrea Rodriguez, Daniel Kopta
    // University of Utah

    /// <summary>
    /// A static class that will wrap an "Evaluate" method which will produce a value calculated through simple arithmetic(infix expressions) on an input string.
    /// </summary> 
    public static class Evaluator
    {
        /// <summary>
        /// Evaluates the input expression consisting of numeric value and variable values. Variable Values are known through the Lookup method.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String expr, Lookup variableEvaluator)
        {
            string[] tokens = configureInput(expr);

            return calculateExpression(tokens, variableEvaluator);
        }

        private static string[] configureInput(string expr)
        {
            if (expr == null)
                throw new ArgumentException(); // throw exception if the input string is null
            string[] substrings = System.Text.RegularExpressions.Regex.Split(expr, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return substrings;
        }
        /// <summary>
        /// Calculates a value from an input expression using provided algorithm template.
        /// </summary>
        /// <param name="substrings"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        private static int calculateExpression(string[] substrings, Lookup variableEvaluator)
        {
            Stack<string> values = new Stack<string>();
            Stack<string> operators = new Stack<string>();
            foreach (string t in substrings)
            {
                string token = t.Trim();
                if (int.TryParse(token, out _) || token.isVar()) // if token is some value
                {
                    if (checkStack(operators, "*", "/")) // if the operator is '*' or '/'
                        tryMultOrDivide(values, token, operators, variableEvaluator);
                    else
                        values.Push(intToString(getValue(token, variableEvaluator)));
                }
                else if (token.Equals("+") || token.Equals("-"))
                {
                    if (checkStack(operators, "+", "-"))
                        tryAddOrSubtract(values, operators, variableEvaluator);
                    operators.Push(token);
                }
                else if (token.Equals("*") || token.Equals("/"))
                    operators.Push(token);

                else if (token.Equals("("))
                    operators.Push(token);

                else if (token.Equals(")"))
                    computeParanth(values, operators, variableEvaluator);
                else if (token.Equals(" ") || token.Equals(""))
                    continue;
                else
                    throw new ArgumentException();
            }
            return outPut(values, operators, variableEvaluator);
        }
        /// <summary>
        /// Checks for 'correctness' in the components of the stacks for final evaluation and returns the final value of the expression. 
        /// if there are no values in the operators stack, then there must be one value in the values stack. Return said value as the solution.
        /// If there exists one operator in the operator stack then said operator must be a "+" or "-" and there must exist exactly two values in 
        /// the value stack. Return the final computation. 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        private static int outPut(Stack<string> values, Stack<string> operators, Lookup variableEvaluator)
        {
            if (operators.isEmpty())
            {
                if (values.Count == 1)
                    return getValue(values.Pop(), variableEvaluator);
                throw new ArgumentException();
            }
            if (operators.Count == 1 && (operators.Peek().Equals("+") || operators.Peek().Equals("-")))
            {
                if (values.Count == 2)
                {
                    string val1 = values.Pop();
                    return addOrSub(values.Pop(), val1, operators.Pop(), variableEvaluator);
                }
                throw new ArgumentException();
            }
            throw new ArgumentException();
        }
        ///<summary>
        ///Attempts to add or subtract values in the expression
        ///</summary>
        private static void tryAddOrSubtract(Stack<string> values, Stack<string> operators, Lookup variableEvaluator)
        {
            if (values.Count < 2)
                throw new ArgumentException();

            string value1 = values.Pop();
            string value2 = values.Pop();
            int calculatedValue = addOrSub(value2, value1, operators.Pop(), variableEvaluator);
            values.Push(intToString(calculatedValue));
        }

        ///<summary>
        ///Attempts to multiply or divide values in the expression
        ///</summary>
        private static void tryMultOrDivide(Stack<string> values, string token, Stack<string> operators, Lookup variableEvaluator)
        {
            int calculatedValue = 0;
            try // try to divide values, throw argument exception if division by zero occurs
            { 
                calculatedValue = (token == null) ? multiplyOrDivide(values.Pop(), values.Pop(), operators.Pop(), variableEvaluator):
                    multiplyOrDivide(values.Pop(), token, operators.Pop(), variableEvaluator);
            }
            catch (DivideByZeroException)
            {
                throw new ArgumentException();
            }
            values.Push(intToString(calculatedValue));
        }
        ///<summary>
        ///computes operations within paranthesis
        /// </summary>
        private static void computeParanth(Stack<string> values, Stack<string> operators, Lookup variableEvaluator)
        {
            if (checkStack(operators, "+", "-"))
            {
                tryAddOrSubtract(values, operators, variableEvaluator);

                if (operators.isEmpty() || !operators.Peek().Equals("("))
                    throw new ArgumentException();
                operators.Pop();
                if (checkStack(operators, "*", "-"))
                {
                    if (values.Count < 2)
                        throw new ArgumentException();

                    tryMultOrDivide(values, null, operators, variableEvaluator);
                }
            }
            else if (checkStack(operators, "(", "("))
                operators.Pop();
            else
                throw new ArgumentException();
        }
        // Checks if the stack has either of the two parameter strings at the top given that the stack is not empty
        private static bool checkStack(Stack<string> operators, string op1, string op2)
        {
            return !operators.isEmpty() && (operators.Peek().Equals(op1) || operators.Peek().Equals(op2));
        }
        // Calculates a value through multiplication or division of the two inputs depending on the corrosponding operator.
        private static int multiplyOrDivide(string int1, string int2, string op, Lookup variableEvaluator)
        {
            return opChoice(int1, int2, op, variableEvaluator, true);
        }
        // Adds or subtracts the two params depending on the corrosponding operator
        private static int addOrSub(string int1, string int2, string op, Lookup variableEvaluator)
        {
            return opChoice(int1, int2, op, variableEvaluator, false);
        }
        // gets the numeric value of a string representation of an integer
        private static int getValue(string token, Lookup variableEvaluator)
        {
            if (token.isVar())
                return variableEvaluator(token);
            return Int32.Parse(token);
        }
        // Wrapper for the arithmetic operations
        private static int opChoice(string int1, string int2, string op, Lookup variableEvaluator, bool isMultOrDivision)
        {
            if (isMultOrDivision)
                return (op.Equals("*")) ? getValue(int1, variableEvaluator) * getValue(int2, variableEvaluator) : getValue(int1, variableEvaluator) / getValue(int2, variableEvaluator);

            return (op.Equals("+")) ? getValue(int1, variableEvaluator) + getValue(int2, variableEvaluator) : getValue(int1, variableEvaluator) - getValue(int2, variableEvaluator);
        }
        // Makes string representation of input
        private static string intToString(int value)
        {
            return value + "";
        }
    }
    // class for useful object extensions
    public static class Extensions
    {
        // returns true if the stack is empty, false otherwise
        public static bool isEmpty(this Stack<string> stack)
        {
            return stack.Count < 1;
        }
        // Checks if a value is a valid variable
        public static bool isVar(this string token)
        {
            System.Text.RegularExpressions.Regex varMatcher = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]+[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (varMatcher.IsMatch(token))
                return true;
            return false;
        }
    }
}
