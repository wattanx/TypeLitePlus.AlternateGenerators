# TypeLite Plus AlternateGenerators

TypeLite Plus is the utility that generates TypeScript definitions from .NET classes.

It's especially useful to keep your TypeScript classes on the client in sync with your POCO classes on the server.

This library provides an AlternateGenerator that inherits from `TsGenerator`

## AlternateGenerators

This library provides the following functions.

- Output Without Namespace

  The `TsWithoutNamespaceGenerator` allows output without namsepace.

- Support for const assertion

  The `EnumAsConstAssertion` outputs enum as a const assertion.

## Usage

- TsWithoutNamespaceGenerator
  template

```TypeLite.tt
<#@ template debug="false" hostspecific="True" language="C#" #>
<#@ assembly name="netstandard.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.Core.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.AlternateGenerators.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.AlternateGenerators.TestModels.dll" #>

<#@ import namespace="TypeLitePlus" #>
<#@ import namespace="TypeLitePlus.AlternateGenerators" #>
<#@ import namespace="TypeLitePlus.AlternateGenerators.TestModels" #>
<#@output extension=".d.ts"#>

<#@include file="Manager.ttinclude"#>
<# var manager = EntityFrameworkTemplateFileManager.Create(this); #>

<# var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator()).AsConstEnums(false).For<Item>(); #>

<# manager.StartNewFile("Output.ts"); #>
<#= ts.Generate() #>
<#
manager.EndBlock();
manager.Process(true);
#>
```

output

```typescript:Output.ts
export enum ItemType {
	Book = 1,
	Music = 10,
	Clothing = 51
}
export interface Item {
	Type: ItemType;
	Id: number;
	Name: string;
}
```

- EnumAsConstAssertion

```TypeLite.tt
<#@ template debug="false" hostspecific="True" language="C#" #>
<#@ assembly name="netstandard.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.Core.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.AlternateGenerators.dll" #>
<#@ assembly name="$(TargetDir)TypeLitePlus.AlternateGenerators.TestModels.dll" #>

<#@ import namespace="TypeLitePlus" #>
<#@ import namespace="TypeLitePlus.AlternateGenerators" #>
<#@ import namespace="TypeLitePlus.AlternateGenerators.Extensions" #>
<#@ import namespace="TypeLitePlus.AlternateGenerators.TestModels" #>
<#@output extension=".d.ts"#>

<#@include file="Manager.ttinclude"#>
<# var manager = EntityFrameworkTemplateFileManager.Create(this); #>

<# var ts = TypeScript.Definitions(new TsWithoutNamespaceGenerator()).EnumAsConstAssertion(true).For<Item>(); #>

<# manager.StartNewFile("Output.ts"); #>
<#= ts.Generate() #>
<#
manager.EndBlock();
manager.Process(true);
#>
```

output

```typescript:Output.ts
export const ItemType = {
	Book: 1,
	Music: 10,
	Clothing: 51
} as const;
export type ItemType = typeof ItemType[keyof typeof ItemType];
export interface Item {
	Type: ItemType;
	Id: number;
	Name: string;
}
```
