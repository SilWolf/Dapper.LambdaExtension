using System.Linq.Expressions;

namespace AZ.Dapper.LambdaExtension.Resolver.ExpressionTree
{
    class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
