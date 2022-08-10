// See https://aka.ms/new-console-template for more information
using static TestProject.ClearQueryFromCustomerData;
using static System.Console;
class Program
{
    static void Main()
    {
        string demoQuery = "Connections| where DestinationEntityType =~'AadIdentity'| extend a = a + b |let x= where a > 10";
        var cleanQuery = ReplaceCustomerDataInQuery(demoQuery);
        if (cleanQuery.GetType() == typeof(string))
        {
            WriteLine("Source query: " + demoQuery);
            WriteLine("New query: " + cleanQuery);
        }
        else
        {
            WriteLine("The query isn't valid. The errors:");
            ((List<string>)cleanQuery).ForEach(x => WriteLine(x));
        }

    }
}
