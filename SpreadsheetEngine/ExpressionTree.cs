// <copyright file="ExpressionTree.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace SpreadsheetEngine;

/// <summary>
/// Expression Tree class for Spreadsheet engine.
/// </summary>
public class ExpressionTree
{
    /// <summary>
    /// Dictionary for variables.
    /// </summary>
    public Dictionary<string, double> variables = new Dictionary<string, double>();

    private readonly Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();

    /// <summary>
    /// Stack to hold the expressions during compilation.
    /// </summary>
    public static Stack<TreeNode> expressionStack = new Stack<TreeNode>();

    /// <summary>
    /// Stack to hold the variables.
    /// </summary>
    public static Stack<string> variableStack = new Stack<string>();

    /// <summary>
    /// root field.
    /// </summary>
    private TreeNode root = null;

    /// <summary>
    /// Expression to be evaluated.
    /// </summary>
    private string expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
    /// </summary>
    /// <param name="expression"></param>
    public ExpressionTree(string expression)
    {
        this.expression = expression;
        if (expression != string.Empty)
        {
            this.UpdatedCompile(expression);
            this.root = this.CompilePostExpression();
        }
    }

    /// <summary>
    /// Gets or sets accessor for the private field "expression".
    /// </summary>
    public string Expression
    {
        get => this.expression;

        set
        {
            this.expression = value;
            this.variables.Clear();
            variableStack.Clear();
            expressionStack.Clear();
        }
    }

    /// <summary>
    /// Method to set include variable values in the Variable Dictionary.
    /// </summary>
    /// <param name="variableName"></param>
    /// <param name="variableValue"></param>
    public void SetVariable(string variableName, double variableValue)
    {
        this.variables[variableName] = variableValue;
    }
    
    public List<string> GetVariables()
    {
        return new List<string>(this.variables.Keys);
    }

    /// <summary>
    /// Helper function for the evaluate method.
    /// </summary>
    /// <returns>evaluated double.</returns>
    public double Evaluate()
    {
        return this.Evaluate(this.root);
    }
    

    /// <summary>
    /// Evaluate method for tree.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>The evaluated result of the expression.</returns>
    /// <exception cref="NotSupportedException"> If the op is not supported.</exception>
    private double Evaluate(TreeNode node)
    {
        ConstantNode? constantNode = node as ConstantNode;
        if (constantNode != null)
        {
            return constantNode.Value;
        }

        // as a variable
        VariableNode? variableNode = node as VariableNode;
        if (variableNode != null)
        {
            if (this.variables.TryGetValue(variableNode.Name, out double value))
            {
                return value;
            }
            else
            {
                // Key not found, return 0.0
                return 0.0;
            }
        }

        // it is an operator node if we came here
        OperatorNode? operatorNode = node as OperatorNode;
        if (operatorNode != null)
        {
            return operatorNode.DoOperation(this.variables);
        }

        throw new NotSupportedException();
    }

    /// <summary>
    /// Compiles and returns the PostFixExpression to a Tree.
    /// </summary>
    /// <returns>Node.</returns>
    private TreeNode CompilePostExpression()
    {
        this.ConvertPostExpression();
        Stack<TreeNode> result = new Stack<TreeNode>();
        TreeNode temp;

        while (expressionStack.Count > 0)
        {
            temp = expressionStack.Pop();

            VariableNode? variable = temp as VariableNode;
            if (variable != null)
            {
                result.Push(variable);
            }

            ConstantNode? con = temp as ConstantNode;
            if (con != null)
            {
                result.Push(con);
            }

            OperatorNode? op = temp as OperatorNode;
            if (op != null)
            {
                op.Right = result.Pop();
                op.Left = result.Pop();

                result.Push(op);
            }
        }

        return result.Pop();
    }

    /// <summary>
    /// changes the post expression so that pop would be grabbing the first element instead of last.
    /// </summary>
    private void ConvertPostExpression()
    {
        Stack<TreeNode> temp = new Stack<TreeNode>();
        while (expressionStack.Count > 0)
        {
            OperatorNode? checker = expressionStack.Peek() as OperatorNode;
            if (checker != null && (checker.Operator == '(' || checker.Operator == ')'))
            {
                throw new Exception("Invalid expression entered.");
            }

            temp.Push(expressionStack.Pop());
        }

        expressionStack = temp;
    }

    /// <summary>
    /// changes the post expression so that pop would be grabbing the first element instead of last.
    /// </summary>
    // ReSharper disable once ParameterHidesMember
#pragma warning disable SA1611
    private void UpdatedCompile(string expression)
#pragma warning restore SA1611
    {
        bool entered = true;
        string pattern = @"([()+\-*\/])"; // Regular expression pattern to match operators
        string[] words = Regex.Split(expression, pattern);

        // removing the white spaces from the list so only operands and operators are left.
        words = words.Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();
        foreach (string word in words)
        {
            if (this.IsOperatorSupported(word) && this.CheckParenthesis(word))
            {
                expressionStack.Push(ExpTreeNodeFactory.CreateVariableOrConstantNode(word));
                entered = false;
            }

            if (word == "(")
            {
                variableStack.Push(word);
            }

            if (word == ")")
            {
                this.PopAndPush();
            }

            if ((this.IsOperatorSupported(word) == false && variableStack.Count == 0) || (this.IsOperatorSupported(word) == false && variableStack.Peek() == "("))
            {
                variableStack.Push(word);
                entered = false;
            }

            if (this.IsOperatorSupported(word) == false && entered)
            {
                if (this.IsOperatorSupported(word))
                {
                    throw new Exception("Operator not supported");
                }

                OperatorNode temp = ExpTreeNodeFactory.CreateOperatorNode(char.Parse(word));
                OperatorNode tstack = ExpTreeNodeFactory.CreateOperatorNode(char.Parse(variableStack.Peek()));
                if ((temp.Precedence > tstack.Precedence) || (temp.Precedence == tstack.Precedence && temp.Associativity == 1))
                {
                    variableStack.Push(word);
                }

                if ((temp.Precedence < tstack.Precedence) || (temp.Precedence == tstack.Precedence && temp.Associativity == 0))
                {
                    this.PopForLeft(temp);
                    variableStack.Push(word);
                }
            }

            entered = true;
        }

        while (variableStack.Count > 0)
        {
            expressionStack.Push(ExpTreeNodeFactory.CreateOperatorNode(char.Parse(variableStack.Pop())));
        }
    }

    /// <summary>
    /// To check and see if an op is supported.
    /// </summary>
    /// <param name="op"></param>
    /// <returns>bool.</returns>
    private bool IsOperatorSupported(string op)
    {
        switch (op)
        {
            case "^":
                return false;
            case "*":
                return false;
            case "/":
                return false;
            case "+":
                return false;
            case "-":
                return false;
        }

        return true;
    }

    /// <summary>
    /// Pops and pushes values until a '(' is found.
    /// Used when a ')' is found.
    /// </summary>
    private void PopAndPush()
    {
        while (variableStack.Count > 0)
        {
            if (variableStack.Peek() == "(")
            {
                variableStack.Pop();
                break;
            }
            else
            {
                expressionStack.Push(ExpTreeNodeFactory.CreateOperatorNode(char.Parse(variableStack.Pop())));
            }
        }
    }

    /// <summary>
    /// Checks if string is a parenthesis.
    /// </summary>
    /// <param name="word">String.</param>
    /// <returns>bool.</returns>
    private bool CheckParenthesis(string word)
    {
        switch (word)
        {
            case "(":
                return false;
            case ")":
                return false;
        }

        return true;
    }

    /// <summary>
    /// Continues to pop from varStck until the precedence is greater than or equal to the one being added.
    /// Every node popped will be added to the result of PostFixExpression expressionStack.
    /// </summary>
    /// <param name="temp">OperatorNode.</param>
    private void PopForLeft(OperatorNode temp)
    {
        while (variableStack.Count > 0)
        {
            if (ExpTreeNodeFactory.CreateOperatorNode(char.Parse(variableStack.Peek())).Precedence >= temp.Precedence)
            {
                expressionStack.Push(ExpTreeNodeFactory.CreateOperatorNode(char.Parse(variableStack.Pop())));
            }
            else
            {
                break;
            }
        }
    }
}