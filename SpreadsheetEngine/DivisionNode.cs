namespace SpreadsheetEngine;

public class DivisionNode : OperatorNode
{

    public DivisionNode()
    {
        this.Operator = '/';
        this.Precedence = 3;
        this.Associativity = 0;
    }

    public override double DoOperation(Dictionary<string, double> variables)
    {
       if(this.Evaluate(this.Right, variables) == 0.0)
       {
           throw new DivideByZeroException("Divide by zero!");
       }
       else
        return this.Evaluate(this.Left, variables) / this.Evaluate(this.Right, variables);

    }
}