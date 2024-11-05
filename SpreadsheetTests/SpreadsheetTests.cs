using SpreadsheetEngine;

namespace SpreadsheetTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    
    /// <summary>
    /// Test Case for the saving functionality of the spreadsheet.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>the value of the cell</returns>
    [Test]
    [TestCase (0,0, ExpectedResult = "Ellie")]
    [TestCase (0,1, ExpectedResult = "Stayner")]
    [TestCase (1,0, ExpectedResult = "is the")]
    [TestCase (1,1, ExpectedResult = "best")]
    public string? TestLoading(int row, int col)
    {
        Spreadsheet temp = new Spreadsheet(2, 2);

        string fileName = "test.xml";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        using (Stream stream = File.OpenRead(filePath))
        {
            temp.LoadSpreadsheet(stream); // Load XML contents into the spreadSheet
        }
        return temp.GetCell(row, col)?.Value;
    }


    [Test]
    [TestCase (0,0, ExpectedResult = "!(selfReference)")]
    [TestCase (1,0, ExpectedResult = "!(selfReference)")]
    [TestCase (1,1, ExpectedResult = "!(selfReference)")]
    public string? TestingSelfReference(int row, int col)
    {
        Spreadsheet demo = new Spreadsheet(2,2);
        
        demo.GetCell(0, 0)!.Text = "=A1";
        demo.GetCell(1, 0)!.Text = "=A2";
        demo.GetCell(1, 1)!.Text = "=B2";

        return demo.GetCell(row, col)?.Value;
    }


    [Test]
    public void TestBadReference()
    {
        Spreadsheet test = new Spreadsheet(2, 2);
        test.GetCell(0, 0)!.Text = "=A123";
        Assert.That(test.GetCell(0, 0)!.Value, Is.EqualTo("!(selfReference)"));
    }
    
}