using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeLitePlus;
using TypeLitePlus.TsModels;

namespace TypeLitePlus.AlternateGenerators
{
    public class TsWithoutNamespaceGenerator : TsGenerator
    {
        protected override void AppendModule(TsModule module, ScriptBuilder sb, TsGeneratorOutput generatorOutput)
        {   
            var classes = module.Classes.Where(c => !InvokeConverterRegisterd(c.Type) && !c.IsIgnored).OrderBy(c => GetTypeName(c)).ToList();
            var baseClasses = classes
                .Where(c => c.BaseType != null)
                .Select(c => c.BaseType.Type.FullName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            var enums = module.Enums.Where(e => !InvokeConverterRegisterd(e.Type) && !e.IsIgnored).OrderBy(e => GetTypeName(e)).ToList();

            if ((generatorOutput == TsGeneratorOutput.Enums && enums.Count == 0) ||
                (generatorOutput == TsGeneratorOutput.Properties && classes.Count == 0) ||
                (enums.Count == 0 && classes.Count == 0))
            {
                return;
            }

            if (generatorOutput == TsGeneratorOutput.Properties && !classes.Any(c => c.Fields.Any() || c.Properties.Any())) { return; }

            if (generatorOutput == TsGeneratorOutput.Constants && !classes.Any(c => c.Constants.Any())) { return; }

            if (IsEnums(generatorOutput))
            {
                foreach (var enumModel in enums)
                {
                    this.AppendEnumDefinition(enumModel, sb, generatorOutput);
                }
            }

            if (IsProperties(generatorOutput) || IsFields(generatorOutput))
            {
                foreach (var baseClassModel in classes.Where(c => baseClasses.Contains(c.Type.FullName)))
                {
                    this.AppendClassDefinition(baseClassModel, sb, generatorOutput);
                }
            }

            if (IsProperties(generatorOutput) || IsFields(generatorOutput))
            {
                foreach (var classModel in classes.Where(c => !baseClasses.Contains(c.Type.FullName)))
                {
                    this.AppendClassDefinition(classModel, sb, generatorOutput);
                }
            }

            if (IsConstants(generatorOutput))
            {
                foreach (var classModel in classes)
                {
                    if (classModel.IsIgnored)
                    {
                        continue;
                    }

                    this.AppendConstantModule(classModel, sb);
                }
            }
        }

        protected override void AppendClassDefinition(TsClass classModel, ScriptBuilder sb, TsGeneratorOutput generatorOutput)
        {
            string typeName = this.GetTypeName(classModel);
            string visibility = "export ";
            _docAppender.AppendClassDoc(sb, classModel, typeName);
            sb.AppendFormatIndented("{0}{1} {2}", visibility, "interface", typeName);

            if (classModel.BaseType != null)
            {
                sb.AppendFormat(" extends {0}", this.GetTypeName(classModel.BaseType));
            }

            if (classModel.Interfaces.Count > 0)
            {
                var implementations = classModel.Interfaces.Select(GetFullyQualifiedTypeName).ToArray();

                var prefixFormat = classModel.Type.IsInterface ? " extends {0}"
                    : classModel.BaseType != null ? " , {0}"
                    : " extends {0}";

                sb.AppendFormat(prefixFormat, string.Join(" ,", implementations));
            }

            sb.AppendLine(" {");

            var members = new List<TsProperty>();
            if (IsProperties(generatorOutput))
            {
                members.AddRange(classModel.Properties);
            }

            if (IsFields(generatorOutput))
            {
                members.AddRange(classModel.Fields);
            }

            using (sb.IncreaseIndentation())
            {
                foreach (var property in members.Where(p => !p.IsIgnored).OrderBy(p => GetTsPropertyType(p)))
                {
                    _docAppender.AppendPropertyDoc(sb, property, GetPropertyName(property), GetTsPropertyType(property));
                    sb.AppendLineIndented(string.Format("{0}: {1};", GetPropertyName(property), GetTsPropertyType(property)));
                }
            }

            sb.AppendLineIndented("}");

            _generatedClasses.Add(classModel);
        }

        private bool InvokeConverterRegisterd(Type type)
        {
            // HACK: _typeConvertors is declared internal.
            // However in this generator, it needs to be used.
            var typeConvertors = GetType().GetField("_typeConvertors", BindingFlags.NonPublic | BindingFlags.Instance);
            var obj = typeConvertors.GetValue(this);
            var method = obj.GetType().GetMethod("IsConvertorRegistered");
            return (bool)method.Invoke(obj, new object[] { type });
        }

        private string GetTsPropertyType(TsProperty property)
        {
            var typeName = GetTypeName(property.PropertyType);
            return _memberTypeFormatter(property, typeName);
        }

        private bool IsEnums(TsGeneratorOutput generatorOutput) => (generatorOutput & TsGeneratorOutput.Enums) == TsGeneratorOutput.Enums;

        private bool IsProperties(TsGeneratorOutput generatorOutput) => (generatorOutput & TsGeneratorOutput.Properties) == TsGeneratorOutput.Properties;

        private bool IsFields(TsGeneratorOutput generatorOutput) => (generatorOutput & TsGeneratorOutput.Fields) == TsGeneratorOutput.Fields;

        private bool IsConstants(TsGeneratorOutput generatorOutput) => (generatorOutput & TsGeneratorOutput.Constants) == TsGeneratorOutput.Constants;
    }
}
