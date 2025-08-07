# EntityFramework.Explained
> Because We Need to Talk About Kevin.


A minimal test catalog for explaining how EF Core behaves in real-world schema output.
Each file isolates a single behavior.

## Folder Structure

```text
EntityFramework.Explained/
├── Conventions/
│   ├── DefaultStringLength.cs
│   ├── RequiredAttribute.cs
│   └── ColumnAttributeOverrides.cs
│
├── Nullability/
│   ├── StringNullability.cs
│   ├── IntNullability.cs
│   ├── ClassNullability.cs
│   └── Etc.cs
│
├── Relationships/
│   ├── OneToMany.cs
│   ├── ManyToMany.cs
│   └── ShadowForeignKeys.cs
│
├── Indexes/
│   ├── DefaultIndexNames.cs
│   └── UniqueConstraints.cs
│
├── Inheritance/
│   ├── TablePerHierarchy.cs
│   ├── TablePerType.cs
│   └── DiscriminatorColumn.cs
│
├── SqlGeneration/
│   ├── IdentityStrategy.cs
│   ├── DefaultConstraints.cs
│   └── ComputedColumns.cs
│
├── _Tools/
│   └── TestContexts/
│       ├── TestSqlServerContext.cs
│       ├── TestSqliteContext.cs
│       └── BaseContext.cs
│
├── EntityFramework.Explained.csproj
```
Etcetera ...

## Guidelines

- Each file = one behavior, one `[DocFile]`
- Each `[Fact]` = one database (e.g. SqlServer, Sqlite)
- Use `GenerateCreateScript()` for schema output
- Keep test classes short, self-contained, with inline models
- Use helper like `AsAssertsToLogFile()` to scaffold output comparison

## Naming suggestions

| Folder         | Focus                        |
|----------------|-------------------------------|
| Conventions    | Data annotations, fluent API |
| Nullability    | `string?` vs `string` etc.    |
| Relationships  | Nav props, FK behavior        |
| Indexes        | Index naming, uniqueness      |
| Inheritance    | TPH/TPT mapping               |
| SqlGeneration  | What actually gets written    |

