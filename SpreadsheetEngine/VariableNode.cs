// <copyright file="VariableNode.cs" company="Osaze Ogieriakhi 11784953">
// Copyright (c) Osaze Ogieriakhi 11784953. All rights reserved.
// </copyright>

namespace SpreadsheetEngine;

/// <summary>
/// Variable Node class.
/// </summary>
public class VariableNode : TreeNode
{
    /// <summary>
    /// Gets or sets name of the variable.
    /// </summary>
    public string Name { get; set; }
}