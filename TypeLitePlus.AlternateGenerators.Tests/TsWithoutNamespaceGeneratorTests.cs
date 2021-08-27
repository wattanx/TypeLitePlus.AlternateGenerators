using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

using TypeLitePlus.AlternateGenerators.TestModels;
using TypeLitePlus.AlternateGenerators.Extensions;

namespace TypeLitePlus.AlternateGenerators.Tests
{
    public class TsWithoutNamespaceGeneratorTests
    {
        private readonly ITestOutputHelper output;

        public TsWithoutNamespaceGeneratorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void WhenClassHasBase_MultipleBasesArentGenerated()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Employee>()
                .For<User>();

            var script = ts.Generate();
            output.WriteLine(script);
            var count = Regex.Matches(script, Regex.Escape("interface Person")).Count;
            Assert.True(count == 1, script);
        }

        [Fact]
        public void WhenModeIsClasses_ClassesArentGenerated()
        {

            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Employee>()
                .For<User>();

            var script = ts.Generate();

            output.WriteLine(script);

            var interfaceCount = Regex.Matches(script, Regex.Escape("interface")).Count;
            Assert.True(interfaceCount > 0, script);

            var namespaceCount = Regex.Matches(script, Regex.Escape("namespace")).Count;
            Assert.True(namespaceCount == 0, script);

            var classCount = Regex.Matches(script, Regex.Escape("class")).Count;
            Assert.True(classCount == 0, script);

            var moduleCount = Regex.Matches(script, Regex.Escape("module")).Count;
            Assert.True(moduleCount == 0, script);
        }

        [Fact]
        public void WhenModeIsInterfaces_ClassesArentGenerated()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Employee>()
                .For<User>();

            var script = ts.Generate();

            output.WriteLine(script);

            var interfaceCount = Regex.Matches(script, Regex.Escape("interface")).Count;
            Assert.True(interfaceCount > 0, script);

            var namespaceCount = Regex.Matches(script, Regex.Escape("namespace")).Count;
            Assert.True(namespaceCount == 0, script);

            var classCount = Regex.Matches(script, Regex.Escape("class")).Count;
            Assert.True(classCount == 0, script);

            var moduleCount = Regex.Matches(script, Regex.Escape("module")).Count;
            Assert.True(moduleCount == 0, script);
        }

        [Fact]
        public void WhenClassIsIgnored_InterfaceForClassIsntGenerated()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();
            model.Classes.Where(o => o.Name == "Address").Single().IsIgnored = true;

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);

            Assert.DoesNotContain("Address", script);
        }

        [Fact]
        public void WhenClassIsIgnoredByAttribute_InterfaceForClassIsntGenerated()
        {
            var builder = new TsModelBuilder();
            builder.Add<IgnoreTestBase>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);
            
            Assert.DoesNotContain("IgnoreTestBase", script);
        }

        [Fact]
        public void WhenBaseClassIsIgnoredByAttribute_InterfaceForClassIsntGenerated()
        {
            var builder = new TsModelBuilder();
            builder.Add<IgnoreTest>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);
            
            Assert.DoesNotContain("interface IgnoreTestBase", script);
        }

        [Fact]
        public void WhenPropertyIsIgnored_PropertyIsExcludedFromInterface()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();
            model.Classes.Where(o => o.Name == "Address").Single().Properties.Where(p => p.Name == "Street").Single().IsIgnored = true;

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);

            Assert.DoesNotContain("Street", script);
        }

        [Fact]
        public void WhenClassIsReferenced_FullyQualifiedNameIsntUsed()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Person>();
            
            var script = ts.Generate();
            output.WriteLine(script);

            Assert.Contains("PrimaryAddress: Address", script);
            Assert.Contains("Addresses: Address[]", script);
        }

        [Fact]
        public void WhenClassIsReferencedAndOutputIsSetToEnums_InterfaceIsntInOutput()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();
            
            var script = ts.Generate(TsGeneratorOutput.Enums);
            output.WriteLine(script);

            Assert.DoesNotContain("interface Item", script);
        }

        [Fact]
        public void WhenClassIsReferencedAndOutputIsSetToEnums_ConstantIsntInOutput()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();

            var script = ts.Generate(TsGeneratorOutput.Enums);
            output.WriteLine(script);

            Assert.DoesNotContain("MaxItems", script);
        }

        [Fact]
        public void WhenEnumIsReferencedAndOutputIsSetToProperties_EnumIsntInOutput()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();

            var script = ts.Generate(TsGeneratorOutput.Properties);
            output.WriteLine(script);

            Assert.DoesNotContain("enum ItemType", script);
        }

        [Fact]
        public void WhenEnumIsReferencedAndOutputIsSetToProperties_ConstantIsntInOutput()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();

            var script = ts.Generate(TsGeneratorOutput.Properties);
            output.WriteLine(script);

            Assert.DoesNotContain("MaxItems", script);
        }

        [Fact]
        public void WhenEnumIsReferencedAndOutputIsSetToFields_EnumIsntInOutput()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();
            var script = ts.Generate(TsGeneratorOutput.Fields);
            output.WriteLine(script);

            Assert.DoesNotContain("enum ItemType", script);
        }

        [Fact]
        public void WhenClassWithEnumReferenced_FullyQualifiedNameIsntUsed()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<Item>();
            var script = ts.Generate();
            output.WriteLine(script);

            Assert.Contains("Type: ItemType", script);
        }

        [Fact]
        public void WhenConvertorIsRegistered_ConvertedTypeNameIsUsed()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            target.RegisterTypeConvertor<string>(type => "KnockoutObservable<string>");
            var script = target.Generate(model);
            output.WriteLine(script);

            Assert.Contains("Street: KnockoutObservable<string>", script);
        }

        [Fact]
        public void WhenConvertorIsRegistered_ConvertedTypeNameIsUsedForFields()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            target.RegisterTypeConvertor<string>(type => "KnockoutObservable<string>");
            var script = target.Generate(model, TsGeneratorOutput.Fields);
            output.WriteLine(script);

            Assert.Contains("PostalCode: KnockoutObservable<string>", script);
        }

        [Fact]
        public void WhenConvertorIsRegisteredForGuid_ConvertedTypeNameIsUsed()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            target.RegisterTypeConvertor<Guid>(type => "string");
            var script = target.Generate(model);
            output.WriteLine(script);

            Assert.Contains("Id: string", script);
        }

        [Fact]
        public void WhenConvertorIsRegisteredForGuid_NoStringInterfaceIsDefined()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            target.RegisterTypeConvertor<Guid>(type => "string");
            var script = target.Generate(model);
            output.WriteLine(script);

            Assert.DoesNotContain("interface string {", script);
        }

        [Fact]
        public void PropertyIsMarkedOptional_OptionalPropertyIsGenerated()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);
            output.WriteLine(script);

            Assert.Contains("CountryID?: number", script);
        }

        [Fact]
        public void WhenInterfaceIsAdded_InterfaceIsInOutput()
        {
            var builder = new TsModelBuilder();
            builder.Add<IShippingService>();
            var model = builder.Build();
            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model, TsGeneratorOutput.Properties);
            output.WriteLine(script);

            Assert.Contains("IShippingService", script);
            Assert.Contains("Price", script);
        }

        [Fact]
        public void WhenEnumModeIsNumber_ValuesAreNumbers()
        {
            var builder = new TsModelBuilder();
            builder.Add<TsEnumModes>();
            var model = builder.Build();
            var target = new TsWithoutNamespaceGenerator();
            target.EnumMode = TsEnumModes.Number;
            var script = target.Generate(model, TsGeneratorOutput.Enums);
            output.WriteLine(script);

            Assert.Contains("Number = 0", script);
        }

        [Fact]
        public void WhenEnumModeIsString_ValuesAreStrings()
        {
            var builder = new TsModelBuilder();
            builder.Add<TsEnumModes>();
            var model = builder.Build();
            var target = new TsWithoutNamespaceGenerator();
            target.EnumMode = TsEnumModes.String;
            var script = target.Generate(model, TsGeneratorOutput.Enums);
            output.WriteLine(script);

            Assert.Contains("Number = \"Number\"", script);
        }

        [Fact]
        public void WhenGenerate_OutputIsFormated()
        {
            var builder = new TsModelBuilder();
            builder.Add<Address>();
            var model = builder.Build();

            var target = new TsWithoutNamespaceGenerator();
            var script = target.Generate(model);
            output.WriteLine(script);

            using (var reader = new StringReader(script))
            {
                var line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("interface Address {"))
                    {
                        Assert.StartsWith("export", line);
                    }
                    if (line.Contains("ID: Guid"))
                    {
                        Assert.StartsWith("\t\t", line);
                    }
                }
            }
        }

        [Fact]
        public void EnumIsGenarateedWithoutModule()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator())
                .For<MyTestEnum>().ToModule("Foo")
                .Generate();
            output.WriteLine(ts);
            Assert.DoesNotContain("namespace Foo", ts);
            Assert.Contains("enum MyTestEnum", ts);
        }

        [Fact]
        public void EnumWithConstAssertion()
        {
            var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator()).EnumAsConstAssertion(true)
                .For<MyTestEnum>().ToModule("Foo")
                .Generate();
            output.WriteLine(ts);
            Assert.DoesNotContain("namespace Foo", ts);
            Assert.Contains("const MyTestEnum", ts);
        }

        enum MyTestEnum
        {
            One,
            Two,
            Three
        }
    }
}
