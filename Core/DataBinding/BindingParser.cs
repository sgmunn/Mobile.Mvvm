// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingParser.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Mvvm.DataBinding
{
    using System;
    using System.Collections.Generic;
	using Mobile.Mvvm.Diagnostics;

    /// <summary>
    /// Parses binding strings into binding expressions
    /// </summary>
    public sealed class BindingParser : IBindingParser
    {
        public readonly static Dictionary<string, IValueConverter> RegisteredConverters = new Dictionary<string, IValueConverter>();

        public static IBindingParser Default = new BindingParser();

        private BindingParser()
        {
        }

        public static void RegisterConverter(IValueConverter converter, string name = null)
        {
            if (RegisteredConverters.ContainsValue(converter))
            {
                return;
            }

            if (name == null)
            {
                name = converter.GetType().Name.Replace("Converter", string.Empty);
            }

            int count = 1;
            var key = name;
            while (RegisteredConverters.ContainsKey(key))
            {
                key = string.Format("{0}_{1}", name, count);
                count++;
            }

            RegisteredConverters.Add(key, converter);
        }

        /// <summary>
        /// Parses the given expression and returns a binding expression against source and target
        /// </summary>
        /// <remarks>
        /// for now the following forms of expression are supported
        /// 
        /// <targetProperty> <bindingMode> <sourceProperty> [ ; ] ...
        /// 
        /// <targetProperty> <bindingMode>  <converterName> ( <sourceProperty> [, <converterParam> ] ) 
        /// 
        /// <bindingMode> =  : | > | <
        /// 
        /// </remarks>
        public IBindingExpression[] Parse(string expression, object target, object source)
        {
            var expressionParts = expression.Split(new [] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var results = new List<IBindingExpression>();
            foreach (var part in expressionParts)
            {
                results.Add(this.ParseSingleExpression(part, target, source));
            }

            return results.ToArray();
        }

        private IBindingExpression ParseSingleExpression(string expression, object target, object source)
        {
            var tokens = this.Tokenise(expression);
            if (tokens.Count != 3 && tokens.Count != 5)
            {
                return null;
            }

            var targetProperty = tokens[0];
            var bindingMode = this.ModeFromToken(tokens[1]);

            var sourceProperty = string.Empty;
            IValueConverter converter = null;
            object convertParam = null;
            
            if (tokens.Count == 3)
            {
                sourceProperty = tokens[2].Trim();
            }
            else
            {
                converter = ConverterFromToken(tokens[2]);
                sourceProperty = tokens[3];
                convertParam = tokens[4];
            }

            var binding = new Binding(sourceProperty);
            binding.Converter = converter;
            binding.ConverterParameter = convertParam;
            binding.Mode = bindingMode;

            return new WeakBindingExpression(target, targetProperty, source, binding);
        }

        private BindingMode ModeFromToken(string mode)
        {
            switch (mode)
            {
                case "<":
                    return BindingMode.OneWay;
                case ">":
                    return BindingMode.OneWayToSource;
            }

            return BindingMode.TwoWay;
        }
        
        private IValueConverter ConverterFromToken(string converter)
        {
            if (RegisteredConverters.ContainsKey(converter))
            {
                return RegisteredConverters[converter];
            }

            Log.Debug("Converter {0} was not registered", converter);

            return null;
        }
        
        private object ConverterParamFromToken(string converterParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes the expression and returns a sequence of tokens
        /// </summary>
        private List<string> Tokenise(string expression)
        {
            // warning, extremely brain dead parsing going on here...
            // for now we're just going to return a sequence of strings

            var tokens = new List<string>();

            var bindingMode = string.Empty;
            if (expression.Contains(":"))
            {
                bindingMode = ":";
            }
            else
            if (expression.Contains("<"))
            {
                bindingMode = "<";
            }
            else
            if (expression.Contains(">"))
            {
                bindingMode = ">";
            }

            // there must be something either side of the bindingMode
            var halves = expression.Split(new [] { bindingMode }, StringSplitOptions.RemoveEmptyEntries);
            if (halves.Length != 2)
            {
                return tokens;
            }

            // the first half is the targetProperty
            tokens.Add(halves[0].Trim());
            tokens.Add(bindingMode);

            // the second half is either a propertyname or a converter expression
            if (halves[1].Contains("("))
            {
                tokens.AddRange(this.TokeniseConverterExpression(halves[1]));
            }
            else
            {
                tokens.Add(halves[1].Trim());
            }

            return tokens;
        }

        private List<string> TokeniseConverterExpression(string expression)
        {
            // <converterName> ( <sourceProperty> [, <converterParam> ] ) 
            var tokens = new List<string>();

            var halves = expression.Split(new [] { "(" }, StringSplitOptions.RemoveEmptyEntries);
            if (halves.Length != 2)
            {
                return tokens;
            }

            tokens.Add(halves[0].Trim());

            var propertyAndParam = halves[1].Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (propertyAndParam.Length == 1)
            {
                // just loose the trailing )
                tokens.Add(halves[1].Replace(")", string.Empty).Trim());
                tokens.Add(null);

                return tokens;
            }

            // ok this next bit is for the convert param and propertyName

            if (halves.Length == 2)
            {
                // propertyName first
                tokens.Add(propertyAndParam[0].Trim());
                // converterParam, loose the ) again
                tokens.Add(propertyAndParam[1].Replace(")", string.Empty).Trim());
            }

            return tokens;
        }
    }
}

