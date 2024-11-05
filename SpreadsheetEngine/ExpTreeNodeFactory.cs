// <copyright file="ExpTreeNodeFactory.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

namespace SpreadsheetEngine;

/// <summary>
/// Factory class to produce specialized cells.
/// </summary>
public static class ExpTreeNodeFactory
{
    /// <summary>
    /// Method defined to produce Operator node based on character passed in.
    /// </summary>
    /// <param name="op"></param>
    /// <returns>OperatorNode.</returns>
    /// <exception cref="NotSupportedException">.</exception>
    public static OperatorNode CreateOperatorNode(char? op)
    {
        if (op != null)
        {
            switch (op)
            {
                case '+':
                    return new AdditionNode();
                case '-':
                    return new SubtractionNode();
                case '*':
                    return new MultiplicationNode();
                case '/':
                    return new DivisionNode();
                default: // if it is not any of the operators that we support, throw an exception:
                    throw new NotSupportedException(
                        "Operator " + op + " not supported.");
            }
        }

        return null!;
    }

    /// <summary>
    /// Method to create variable and constant nodes.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>TreeNode.</returns>
    public static TreeNode CreateVariableOrConstantNode(string value)
    {
        double number;
        bool isNumber = double.TryParse(value, out number);
        if (isNumber)
        {
            return new ConstantNode()
            {
                Value = number,
            };
        }
        else
        {
            return new VariableNode()
            {
                Name = value,
            };
        }
    }
}