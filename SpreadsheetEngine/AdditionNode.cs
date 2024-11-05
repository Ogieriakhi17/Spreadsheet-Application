namespace SpreadsheetEngine;

public class AdditionNode : OperatorNode
{
    public AdditionNode()
    {
        this.Operator = '+';
        this.Precedence = 2;
        this.Associativity = 0;

    }
    public override double DoOperation(Dictionary<string, double> variables)
    {
        return this.Evaluate(this.Left, variables) + this.Evaluate(this.Right, variables);
    }
}