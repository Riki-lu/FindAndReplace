using Kusto.Language;
using Kusto.Language.Syntax;
namespace TestProject
{
    public static class Algorithm
    {

        /// <summary>
        /// wrapper function-call the all functions in order to find and replace the Customer Data
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>if the query is valid, return a clean query-without Customer Data.
        /// if the query invalid return a message-"This query is invalid"</returns>
        public static string MainFunction(string query)
        {
            //check validation to the query
            //the example in the qurey is nit 
            if (!Validation(query))
            {
                //find all Customer data in the query.
                List<string> customerDataWords = PassQueryFindCustomerData(query);
                //if there was Customer data in the query, replace it and return the clean query.
                return customerDataWords.Count > 0 ? ReplaceAllCustomerData(query, customerDataWords) : query;
            }
            return "This query is invalid";
        }

        /// <summary>
        /// Validation checks to the query
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>true if the query is correct and false if not</returns>
        private static bool Validation(string query)
        {
            if (query == null)
                return false;
            //func GetDiagnostics return the errors in a kql query
            var diagnostics = KustoCode.ParseAndAnalyze(query).GetDiagnostics();
            return diagnostics.Count == 0;
        }

        /// <summary>
        /// pass the query, find the sensitve code.
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>list of all customer data had found</returns>
        private static List<string> PassQueryFindCustomerData(string query)
        {
            //list of all customer data
            List<string> CustomerDataWords = new();
            //Convert the query to KustoCode type in order to prase it
            var code = KustoCode.Parse(query);
            //Pass the query. The WalkNodes function splits the query into SyntaxNodes
            //An inspection will be carried out if the specific SyntaxNodes might contain Customer Data
            SyntaxElement.WalkNodes(code.Syntax,
                n =>
                {
                    {
                        switch (n.Kind)
                        {
                            //Sensitive Operators-contain Customer Data
                            //each Node operator represents root of tree, the first Descendant is the Customer Data word.

                            case SyntaxKind.ExtendOperator:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;
                            case SyntaxKind.LetStatement:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;
                            case SyntaxKind.LookupOperator:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;
                            case SyntaxKind.SummarizeOperator:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;
                            case SyntaxKind.ProjectRenameOperator:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;
                            case SyntaxKind.AsOperator:
                                CustomerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                                break;


                            //Sensitive Parmeters-themselvs Customer Data word.

                            case SyntaxKind.NamedParameter:
                                CustomerDataWords.Add(n.ToString());
                                break;
                            case SyntaxKind.StringLiteralExpression:
                                CustomerDataWords.Add(n.ToString());
                                break;
                            case SyntaxKind.FunctionCallExpression:
                                //check if a function declaration called a customer data
                                CustomerDataWords.Add(n.ToString());
                                break;
                            case SyntaxKind.SkippedTokens:
                                //to check 
                                break;
                        }
                    }
                });
            return CustomerDataWords;
        }
            /// <summary>
            /// Replace the customer data words 
            /// </summary>
            /// <param name="query">a Kusto query</param>
            /// <param name="customerDataWords">list of all customer data had found in this query</param>
            /// <returns>new query without customer data</returns>
        public static string ReplaceAllCustomerData(string query, List<string> customerDataWords)
        {
            var index = 0;
            return customerDataWords.Aggregate(query, (str, cItem) => str.Replace(cItem, " CD" + index++));
        }

        //new features c#-switch
        private static string GetCustomerDataWord(SyntaxNode n) => n.Kind switch
        {
            //Sensitive Operators-contain Customer Data
            //each Node operator represents root of tree, the first Descendant is the Customer Data word.
            SyntaxKind.ExtendOperator => n.GetFirstDescendant<NameDeclaration>().ToString(),
            SyntaxKind.LetStatement => n.GetFirstDescendant<NameDeclaration>().ToString(),
            SyntaxKind.SummarizeOperator => n.GetFirstDescendant<NameDeclaration>().ToString(),
            SyntaxKind.ProjectRenameOperator => n.GetFirstDescendant<NameDeclaration>().ToString(),
            SyntaxKind.AsOperator => n.GetFirstDescendant<NameDeclaration>().ToString(),

            //Sensitive Parmeters-themselvs Customer Data word.
            SyntaxKind.NamedParameter => n.ToString(),
            SyntaxKind.StringLiteralExpression => n.ToString(),
            //check if a function declaration called a customer data
            SyntaxKind.FunctionCallExpression => n.ToString(),
            SyntaxKind.SkippedTokens => n.ToString(),

        };

    };

}

