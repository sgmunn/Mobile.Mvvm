// http://rx.codeplex.com/SourceControl/latest#Rx.NET/Source/System.Reactive.Linq/Reactive/Internal/ReflectionUtils.cs

using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace Mobile.Mvvm.DataBinding
{
    // This class was inspired and influenced by the excellent binding work
    // by https://github.com/reactiveui/ReactiveUI/
    // Inspiration used under Microsoft Public License Ms-PL
    public class MvxPropertyExpressionParser //: IMvxPropertyExpressionParser
    {
        public MvxParsedExpression Parse<TObj, TRet>(Expression<Func<TObj, TRet>> propertyPath)
        {
            return Parse((LambdaExpression) propertyPath);
        }

        public MvxParsedExpression Parse(LambdaExpression propertyPath)
        {
            var toReturn = new MvxParsedExpression();

            var current = propertyPath.Body;
            while (current != null
                   && current.NodeType != ExpressionType.Parameter)
            {
                current = ParseTo(current, toReturn);
            }

            return toReturn;
        }

        private static Expression ParseTo(Expression current, MvxParsedExpression toReturn)
        {
            // This happens when a value type gets boxed
            if (current.NodeType == ExpressionType.Convert || current.NodeType == ExpressionType.ConvertChecked)
            {
                return Unbox(current);
            }

            if (current.NodeType == ExpressionType.MemberAccess)
            {
                return ParseProperty(current, toReturn);
            }

            if (current is MethodCallExpression)
            {
                return ParseMethodCall(current, toReturn);
            }

            throw new ArgumentException(
                "Property expression must be of the form 'x => x.SomeProperty.SomeOtherProperty'");
        }

        private static Expression ParseMethodCall(Expression current, MvxParsedExpression toReturn)
        {
            var me = (MethodCallExpression) current;
            if (me.Method.Name != "get_Item"
                || me.Arguments.Count != 1)
            {
                throw new ArgumentException(
                    "Property expression must be of the form 'x => x.SomeProperty.SomeOtherProperty' or 'x => x.SomeCollection[0].Property'");
            }
            var argument = me.Arguments[0];
            toReturn.PrependIndexed(argument.ToString());
            current = me.Object;
            return current;
        }

        private static Expression ParseProperty(Expression current, MvxParsedExpression toReturn)
        {
            var me = (MemberExpression) current;
            toReturn.PrependProperty(me.Member.Name);
            current = me.Expression;
            return current;
        }

        private static Expression Unbox(Expression current)
        {
            var ue = (UnaryExpression) current;
            current = ue.Operand;
            return current;
        }
    }

    public class MvxParsedExpression //: IMvxParsedExpression
    {
        public interface INode
        {
            void AppendPrintTo(StringBuilder builder);
        }

        public class PropertyNode : INode
        {
            public PropertyNode(string propertyName)
            {
                PropertyName = propertyName;
            }

            public string PropertyName { get; private set; }

            public void AppendPrintTo(StringBuilder builder)
            {
                if (builder.Length > 0)
                    builder.Append(".");

                builder.Append(PropertyName);
            }
        }

        public class IndexedNode : INode
        {
            public IndexedNode(string indexValue)
            {
                IndexValue = indexValue;
            }

            public string IndexValue { get; private set; }

            public void AppendPrintTo(StringBuilder builder)
            {
                builder.AppendFormat("[{0}]", IndexValue);
            }
        }

        private readonly LinkedList<INode> _nodes = new LinkedList<INode>();

        protected LinkedList<INode> Nodes
        {
            get { return _nodes; }
        }

        protected void Prepend(INode node)
        {
            _nodes.AddFirst(node);
        }

        public void PrependProperty(string propertyName)
        {
            Prepend(new PropertyNode(propertyName));
        }

        public void PrependIndexed(string indexedValue)
        {
            Prepend(new IndexedNode(indexedValue));
        }

        public string Print()
        {
            var output = new StringBuilder();
            foreach (var node in Nodes)
            {
                node.AppendPrintTo(output);
            }
            return output.ToString();
        }
    }


}

