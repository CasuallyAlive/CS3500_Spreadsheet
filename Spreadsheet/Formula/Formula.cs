// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
// (Jordy A. Larrea Rodriguez - U1236145)
// (Version 1.3 and onwards) Implemented the class based on provided outlines.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<String> tokens;
        private Func<String, String> normalizer;
        private Func<String, bool> isValid;

        private const String emptyFormula = "The formula provided does not meet the requirements: EMPTY FORMULA";
        private const String invalidValue = "The formula provided does not meet the requirements: INVALID CHARACTER ";
        private const String SyntaxError = "The formula provided does not meet the requirements: SYNTAX ERROR - ";
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            this.normalizer = normalize;
            this.isValid = isValid;
            this.tokens = GetTokens(formula).ToList();
            this.CheckSyntaxAndNormalize();
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            try
            {
                return tryComputation(lookup);

            }
            catch (ArgumentException)
            {
                return new FormulaError("Undefined variable(s)");
            }
            catch (DivideByZeroException)
            {
                return new FormulaError("Divide by zero error");
            }
        }
        /// <summary>
        /// Helper method for the Evaluate method, throws ArgumentException if the formula is not correct. Divide-by-zero, etc; otherwise, returns the final output.
        /// </summary>
        /// <param name="lookup"></param>
        /// <returns></returns>
        public object tryComputation(Func<string, double> lookup)
        {
            Stack<string> values = new Stack<string>();
            Stack<string> operators = new Stack<string>();
            foreach (string token in this.tokens)
            {
                if (Double.TryParse(token, out _) || token.isVar()) // if token is some value
                {
                    if (checkStack(operators, "*", "/")) 
                        tryMultOrDivide(values, token, operators, lookup);
                    else
                        values.Push(doubleToString(getValue(token, lookup)));
                }
                else if (token.Equals("+") || token.Equals("-"))
                {
                    if (checkStack(operators, "+", "-"))
                        tryAddOrSubtract(values, operators, lookup);
                    operators.Push(token);
                }
                else if (token.Equals("*") || token.Equals("/"))
                    operators.Push(token);

                else if (token.Equals("("))
                    operators.Push(token);

                else if (token.Equals(")"))
                    computeParanth(values, operators, lookup);
            }
            return outPut(values, operators, lookup);
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
        private double outPut(Stack<string> values, Stack<string> operators, Func<string, double> variableEvaluator)
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
        private void tryAddOrSubtract(Stack<string> values, Stack<string> operators, Func<string, double> variableEvaluator)
        {
            if (values.Count < 2)
                throw new ArgumentException();

            string value1 = values.Pop();
            string value2 = values.Pop();
            double calculatedValue = addOrSub(value2, value1, operators.Pop(), variableEvaluator);
            values.Push(doubleToString(calculatedValue));
        }

        ///<summary>
        ///Attempts to multiply or divide values in the expression
        ///</summary>
        private void tryMultOrDivide(Stack<string> values, string token, Stack<string> operators, Func<string, double> variableEvaluator)
        {
            double calculatedValue = (token == null) ? multiplyOrDivide(values.Pop(), values.Pop(), operators.Pop(), variableEvaluator) :
                    multiplyOrDivide(values.Pop(), token, operators.Pop(), variableEvaluator);
            // try to divide values, throw argument exception if division by zero occurs
            if (Double.IsInfinity(calculatedValue))
                throw new DivideByZeroException();
            values.Push(doubleToString(calculatedValue));
        }
        ///<summary>
        ///computes operations within paranthesis
        /// </summary>
        private void computeParanth(Stack<string> values, Stack<string> operators, Func<string, double> variableEvaluator)
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
        /// <summary>
        /// Checks if the stack has either of the two parameter strings at the top of itself given that the stack is not empty.
        /// </summary>
        /// <param name="operators"></param>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        private bool checkStack(Stack<string> operators, string op1, string op2)
        {
            return !operators.isEmpty() && (operators.Peek().Equals(op1) || operators.Peek().Equals(op2));
        }
        /// <summary>
        /// Calculates a value through multiplication or division of the two inputs depending on the corrosponding operator.
        /// </summary>
        /// <param name="double1"></param>
        /// <param name="double2"></param>
        /// <param name="op"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        private double multiplyOrDivide(string double1, string double2, string op, Func<string, double> variableEvaluator)
        {
            return opChoice(double1, double2, op, variableEvaluator, true);
        }
        /// <summary>
        /// Adds or subtracts the two params depending on the corrosponding operator
        /// </summary>
        /// <param name="double1"></param>
        /// <param name="double2"></param>
        /// <param name="op"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        private double addOrSub(string double1, string double2, string op, Func<string, double> variableEvaluator)
        {
            return opChoice(double1, double2, op, variableEvaluator, false);
        }
        /// <summary>
        /// gets the numeric value of a string representation of an integer
        /// </summary>
        /// <param name="token"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        private double getValue(string token, Func<string, double> variableEvaluator)
        {
            if (token.isVar())
                return variableEvaluator(token);
            return Double.Parse(token);
        }
        /// <summary>
        /// Wrapper for the arithmetic operations
        /// </summary>
        /// <param name="int1"></param>
        /// <param name="int2"></param>
        /// <param name="op"></param>
        /// <param name="variableEvaluator"></param>
        /// <param name="isMultOrDivision"></param>
        /// <returns></returns>
        private double opChoice(string int1, string int2, string op, Func<string, double> variableEvaluator, bool isMultOrDivision)
        {
            if (isMultOrDivision)
                return (op.Equals("*")) ? getValue(int1, variableEvaluator) * getValue(int2, variableEvaluator) : getValue(int1, variableEvaluator) / getValue(int2, variableEvaluator);

            return (op.Equals("+")) ? getValue(int1, variableEvaluator) + getValue(int2, variableEvaluator) : getValue(int1, variableEvaluator) - getValue(int2, variableEvaluator);
        }
        /// <summary>
        /// Makes string representation of input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string doubleToString(double value)
        {
            return value + "";
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> variables = new HashSet<string>();
            foreach(string token in tokens)
            {
                if (token.isVar())
                    variables.Add(token);
            }
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String formula = "";
            foreach(string token in tokens)
            {
                formula += token;
            }
            return formula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Formula) || object.ReferenceEquals(obj,null))
                return false;
            Formula f2 = (Formula)obj;
            return this.GetHashCode() == f2.GetHashCode();
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (Object.ReferenceEquals(f1,null) || Object.ReferenceEquals(f2, null))
                return Object.ReferenceEquals(f1, f2);
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach(string token in tokens)
            {
                double value;
                hashCode += (Double.TryParse(token, out value)) ? value.GetHashCode() : token.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Given a list of tokens, checks if any given token is valid according to syntactic rules and normalizes any variables.
        /// </summary>
        public void CheckSyntaxAndNormalize()
        {
            if (tokens.Count < 1)  
                throw new FormulaFormatException(emptyFormula);
            string token = tokens[0];
            FormulaSyntax formulaChecker = new FormulaSyntax(token, null, null, 0, 0);
            for (int i = 0; i < tokens.Count; i++)
            {
                token = tokens[i];
                if (token.isVar())
                {
                    token = normalizer(token); // Normalizes a variable and checks it with the validation delegate for correctness. 
                    if (!isValid(token))
                        throw new FormulaFormatException(invalidValue + token);
                    tokens[i] = token;
                }
                formulaChecker.setCurrent(token); // updates the current token in the formula checker
                if (i == tokens.Count - 1)
                {
                    if (!formulaChecker.checkCurrent(true))
                        throw new FormulaFormatException(invalidValue + token);
                }
                else if (!formulaChecker.checkCurrent(false))
                    throw new FormulaFormatException(invalidValue + token);
            }
        }

        /// <summary>
        /// Struct that checks for correct syntax by "remembering" the previous and current tokens in a formula and ensures correctness from paranthesis tokens. 
        /// </summary>
        private struct FormulaSyntax
        {
            public string current;
            public string previous;
            public string initialVal;

            public int openParanthesis;
            public int closingParanthesis;

            /// <summary>
            /// Checks if the initial token is either an opening paranthesis, a number, or a variable
            /// </summary>
            private bool checkInitialToken()
            {
                if (this.initialVal.Equals("("))
                    return true;
                if (this.initialVal.isVar())
                    return true;
                if (Double.TryParse(this.initialVal, out _))
                    return true;
                return false;
            }
            /// <summary>
            /// Constructor for the formula-syntax struct.
            /// </summary>
            /// <param name="initialValue"></param>
            /// <param name="previous"></param>
            /// <param name="current"></param>
            /// <param name="openingP"></param>
            /// <param name="closingP"></param>
            public FormulaSyntax(string initialValue, string previous, string current, int openingP,int closingP)
            {
                this.initialVal = initialValue;
                this.previous = previous;
                this.current = current;
                this.openParanthesis = openingP;
                this.closingParanthesis = closingP;
            }
            /// <summary>
            /// Updates the current and previous values in this instance
            /// </summary>
            /// <param name="newCurrent"></param>
            public void setCurrent(string newCurrent)
            {
                if (isOpenParanthesis(newCurrent))
                    openParanthesis++;
                else if (isClosingParanthesis(newCurrent))
                    closingParanthesis++;

                this.previous = (this.previous == null && this.current != null) ? initialVal: this.current;
                this.current = (this.current == null) ? this.initialVal: newCurrent;
            }
            /// <summary>
            /// Returns true if the count of opening paranthesis seen so far is greater than or equal to the count of closing paranthesis
            /// </summary>
            /// <returns></returns>
            /// <summary>
            /// Checks if the current token follows proper syntax in respect to the previous token.
            /// </summary>
            /// <returns></returns>
            public bool checkCurrent(bool isLastToken)
            {
                if (isLastToken)
                    return (closingParanthesis == openParanthesis) && CheckPrevious() && (Double.TryParse(current, out _) || current.isVar() || isClosingParanthesis(current));
                return CheckPrevious(); 
            }
            /// <summary>
            /// Checks for correct syntax in reference to the previous token added to this formula Checker
            /// </summary>
            private bool CheckPrevious() 
            {
                if (previous == null)
                    return checkInitialToken();
                if (isOpenParanthesis(this.previous) || isOperator(this.previous))
                    return (closingParanthesis <= openParanthesis) &&
                        (current.isVar() || Double.TryParse(current, out _) || isOpenParanthesis(current));
                if (Double.TryParse(previous, out _) || previous.isVar() || isClosingParanthesis(previous))
                    return (closingParanthesis <= openParanthesis) &&
                        isOperator(current) || isClosingParanthesis(current);
                return false;
            }

            private bool isOpenParanthesis(string token)
            {
                return token.Equals("(");
            }
            private bool isOperator(string token)
            {
                return token.Equals("*") || token.Equals("/") || token.Equals("+") || token.Equals("-");
            }
            private bool isClosingParanthesis(string token)
            {
                return token.Equals(")");
            }
        }

    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
    /// <summary>
    /// Static class for handy extensions.
    /// </summary>
    public static class Extensions
    {
        // returns true if the stack is empty, false otherwise
        public static bool isEmpty(this Stack<string> stack)
        {
            return stack.Count < 1;
        }
        /// <summary>
        /// Returns true if the token is any letter or underscore followed by any number of letters and/or digits and/or underscores would form valid variable names.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool isVar(this string token)
        {
            Regex varMatcher = new Regex(@"^(?:[a-zA-Z]|[_]|$)+(?:[a-zA-Z]|[0-9]|[_]|$)+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return varMatcher.IsMatch(token);
        }
    }
}


