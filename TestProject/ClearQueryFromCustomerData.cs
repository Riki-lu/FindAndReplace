using Kusto.Language;
using Kusto.Language.Syntax;
namespace TestProject
{
    public static class ClearQueryFromCustomerData
    {
        /// <summary>
        /// wrapper function-call the all functions in order to find and replace the Customer Data
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>if the query is valid, return a clean query-without Customer Data.
        /// if the query invalid return a message-"This query is invalid"</returns>
        public static object ReplaceCustomerDataInQuery(string query)
        {
            var errors = ValidateQuery(query);
            if (errors == null)
            {
                var customerDataWords = PassQueryFindCustomerData(query);
                return customerDataWords.Count > 0 ? ReplaceCustomerData(query, customerDataWords) : query;
            }
            return errors.Select(x => x.Message).ToList();
        }

        /// <summary>
        /// Validation checks to the query
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>true if the query is valid and false if not</returns>
        private static IReadOnlyList<Diagnostic>? ValidateQuery(string query)
        {
            return query == null ? null : KustoCode.ParseAndAnalyze(query).GetDiagnostics();
        }

        /// <summary>
        /// pass the query, find the customer data.
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <returns>list of all customer data had found</returns>
        private static List<string> PassQueryFindCustomerData(string query)
        {
            var customerDataWords = new List<string>();
            var code = KustoCode.Parse(query);
            SyntaxElement.WalkNodes(code.Syntax,
                n =>
                {
                    switch (n.Kind)
                    {
                        //Sensitive Operators-contain Customer Data
                        //each Node operator represents root of tree, the first Descendant is the Customer Data word.

                        case SyntaxKind.ExtendOperator:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;
                        case SyntaxKind.LetStatement:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;
                        case SyntaxKind.LookupOperator:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;
                        case SyntaxKind.SummarizeOperator:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;
                        case SyntaxKind.ProjectOperator:
                            var lstCustomerData = n.GetDescendants<NameDeclaration>();
                            foreach (var item in lstCustomerData)
                            {
                                customerDataWords.Add(item.ToString());
                            }
                            break;
                        case SyntaxKind.ProjectRenameOperator:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;
                        case SyntaxKind.AsOperator:
                            customerDataWords.Add(n.GetFirstDescendant<NameDeclaration>().ToString());
                            break;


                        //Sensitive Parmeters-themselvs Customer Data word.

                        case SyntaxKind.NamedParameter:
                            customerDataWords.Add(n.ToString());
                            break;
                        case SyntaxKind.StringLiteralExpression:
                            customerDataWords.Add(n.ToString());
                            break;
                        case SyntaxKind.FunctionCallExpression:
                            customerDataWords.Add(n.ToString());
                            break;
                    }
                });
            return customerDataWords;
        }
        /// <summary>
        /// Replace the customer data words 
        /// </summary>
        /// <param name="query">a Kusto query</param>
        /// <param name="customerDataWords">list of all customer data had found in this query</param>
        /// <returns>new query without customer data</returns>
        public static string ReplaceCustomerData(string query, List<string> customerDataWords)
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

