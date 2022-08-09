// See https://aka.ms/new-console-template for more information
using static TestProject.Algorithm;
class Program
{
    static void Main()
    {
        string demoQuery = "Connections| where DestinationEntityType =~'AadIdentity'| extend a = a + b |let x= where a > 10";
        var cleanQuery = MainFunction(demoQuery);
        Console.WriteLine("Source query: "+demoQuery);
        Console.WriteLine("New query: " + cleanQuery);
    }
}
