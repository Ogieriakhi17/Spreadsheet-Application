using System.Linq.Expressions;
using SpreadsheetEngine;

namespace ExpressionTreeTests;

/// <summary>
/// Test Functions for Expression tree
/// </summary>
public class Tests
{

    /// <summary>
    /// Functions to test normal cases of the tree evaluating expressions
    /// </summary>
    /// <param name="expression"></param>
    /// <returns> double</returns>
    [Test]
    [TestCase("3+5", ExpectedResult = 8.0)]
    [TestCase ("8-4", ExpectedResult = 4.0)]
    [TestCase ("2*2", ExpectedResult = 4.0)]
    [TestCase ("4+4+3", ExpectedResult = 11.0)]
    [TestCase ("5-4-1", ExpectedResult = 0.0)]
    public double TestNormalCases(string expression)
    {
        ExpressionTree testInput = new ExpressionTree(expression);

        return testInput.Evaluate();

    }

    /// <summary>
    /// Functions to test the variable implementations fo the expression tree
    /// </summary>
    /// <returns> void </returns>
    [Test]
    public void TestVariableInputs()
    {
        ExpressionTree variableInput = new ExpressionTree("a+b");
        Assert.That(variableInput.Expression, Is.EqualTo("a+b"));
        variableInput.SetVariable("a", 4.5);
        variableInput.SetVariable("b", 1.0);
        Assert.That(variableInput.variables.Count, Is.EqualTo(2.0));
        Assert.That(variableInput.Evaluate(), Is.EqualTo(5.5));

    }
    
    /// <summary>
    /// Functions to test normal cases of the tree evaluating expressions supporting
    /// parenthesis and precedence 
    /// </summary>
    /// <param name="expression"></param>
    /// <returns> double</returns>
    [Test]
    [TestCase("(3+5)-2", ExpectedResult = 6.0)]
    [TestCase ("9", ExpectedResult = 9.0)]
    [TestCase ("(2*2)/2", ExpectedResult = 2.0)]
    [TestCase ("(4+4)+(3-2)", ExpectedResult = 9.0)]
    [TestCase ("5-2+5*3", ExpectedResult = 18.0)]
    public double TestAddedSupportedCases(string expression)
    {
        ExpressionTree testInput = new ExpressionTree(expression);

        return testInput.Evaluate();

    }
}