// <copyright file="OperatorNode.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

namespace SpreadsheetEngine;

/// <summary>
/// Operator Node method.
/// </summary>
public abstract class OperatorNode : TreeNode
{
    public OperatorNode()
    {
        this.Left = this.Right = null;
    }

    public int Precedence { get; set; }

    public int Associativity { get; set; }

    /// <summary>
    /// Gets or sets Operator.
    /// </summary>
    public char Operator { get; set; }

    /// <summary>
    /// Gets or sets Left Node.
    /// </summary>
    public TreeNode Left { get; set; }

    /// <summary>
    /// Gets or sets Right node.
    /// </summary>
    public TreeNode Right { get; set; }

    public abstract double DoOperation(Dictionary<string, double> variables);

    public double Evaluate(TreeNode node, Dictionary<string, double> variables)
    {
        // try to evaluate the node as a constant
        // the "as" operator is evaluated to null 
        // as opposed to throwing an exception
        ConstantNode constantNode = node as ConstantNode;
        if (constantNode != null)
        {
            return constantNode.Value;
        }

        // as a variable
        VariableNode variableNode = node as VariableNode;
        if (variableNode != null)
        {
            if (variables.TryGetValue(variableNode.Name, out double value))
            {
                return value;
            }
            else
            {
                // the variable is not found in the dictionary, return 0.0
                return 0.0;
            }
        }

        // it is an operator node if we came here
        OperatorNode operatorNode = node as OperatorNode;
        if (operatorNode != null)
        {
            return operatorNode.DoOperation(variables);
        }

        throw new NotSupportedException();
    }
}