
using SpreadsheetEngine;

namespace ExpressionTreeDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ExpressionTree mainTree = new ExpressionTree("A1+B1+C2");
            bool endChoice = false;
            string choice; 
            
            do
            {
                Console.WriteLine($"**MENU** (current expression = {mainTree.Expression})\n" +
                   "1. Enter a new expression\n" +
                   "2. Set a variable value\n" +
                   "3. Evaluate expression\n" +
                   "4. Quit\n");

                choice = (Console.ReadLine());
                int intValue = int.Parse(choice);
                switch (intValue)
                {
                    case 1:
                        Console.WriteLine("Enter an expression.");
                        string expression = Console.ReadLine();
                        mainTree = new ExpressionTree(expression);
                        break;
                    case 2:
                        Console.WriteLine("Please enter the new variable name");
                        string newVariable = Console.ReadLine();
                        Console.WriteLine("please enter the variable value");
                        double variableValue = Convert.ToDouble(Console.ReadLine());
                        mainTree.SetVariable(newVariable, variableValue);
                        break;
                    case 3:

                        Console.WriteLine(mainTree.Evaluate());
                        break;
                    case 4:
                        Console.WriteLine("Bye.");
                        endChoice = true;
                        break;
                    
                    
                }

            } while (endChoice == false);
        } 
        
        
        
        
    }
    
    
}
