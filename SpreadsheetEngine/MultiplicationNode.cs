// <copyright file="MultiplicationNode.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

namespace SpreadsheetEngine;

/// <summary>
/// Class for the mulitplication class.
/// </summary>
public class MultiplicationNode : OperatorNode
{
    /// <inheritdoc />
    public MultiplicationNode()
    {
        this.Operator = '*';
        this.Precedence = 4;
        this.Associativity = 1;
    }

    /// <summary>
    /// Operation function for multiplication operation nodes.
    /// </summary>
    /// <param name="variables"></param>
    /// <returns> double. </returns>
    public override double DoOperation(Dictionary<string, double> variables)
    {
        return this.Evaluate(this.Left, variables) * this.Evaluate(this.Right, variables);
    }
}