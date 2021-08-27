using System;
using System.Collections.Generic;
using System.Text;

namespace TypeLitePlus.AlternateGenerators.Extensions
{
    public static class TypeScriptFluentExtensions
    {
        public static TypeScriptFluent EnumAsConstAssertion(this TypeScriptFluent ts, bool value)
        {
            if (ts.ScriptGenerator is TsWithoutNamespaceGenerator generator) 
            {
                generator.EnumAsConstAssertion = value;
                return ts;
            }
            return ts;
        }
    }
}
