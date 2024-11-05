// <copyright file="SubtractionNode.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

namespace SpreadsheetEngine;

/// <summary>
/// Subraction class inheriting from the Operator Node.
/// </summary>
public class SubtractionNode : OperatorNode
{
    /// <inheritdoc />
    public SubtractionNode()
    {
        this.Operator = '-';
        this.Precedence = 2;
        this.Associativity = 0;
    }

    /// <summary>
    /// Operation method being overloaded for the subtraction class.
    /// </summary>
    /// <param name="variables"></param>
    /// <returns>double.</returns>
    public override double DoOperation(Dictionary<string, double> variables)
    {
        return this.Evaluate(this.Left, variables) - this.Evaluate(this.Right, variables);
    }
}