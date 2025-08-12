# EntityFramework.Explained
> Because We Need to Talk About Kevin.
#### Dependencies
* [QuickPulse.Explains](https://github.com/kilfour/QuickPulse.Explains)
* [QuickPulse.Show](https://github.com/kilfour/QuickPulse.Show)
## Runtime Behaviour
## Schema
### Column Attributes Overrides
#### Sql Server
Generates table with chosen column names of the right type in the given order.
#### Sqlite
Generates table with chosen column names of the right type (look at date :O) in the given order.
### Data Annotations: `[Range(...)]`
**Given:**
```csharp
public class Thing
{
    public int Id { get; set; }
    [Range(0, 10)] // <= We are checking this 
    public int SecondInt { get; set; }
}
```
#### Sql Server
`[Range(0,10)]` gets ignored : `[SecondInt] int NOT NULL`.
#### Sqlite
Same behaviour as Sql Server: `"SecondInt" INTEGER NOT NULL`.
### Default Index Names
#### Sql Server
has entity with a default index
#### Sql Server
has entity with combined sorted index
#### Sqlite
has entity with a default index
#### Sqlite
has entity with combined sorted index
### Discriminator Column
#### Sql Server
has tph with AnimalType as discriminator and inherited properties of base class and properties of derived classes. Property with type bool will generate an int in the database in SQL Server (output 0 or 1). So we used type string instead.
#### Sqlite
has tph with AnimalType as discriminator and inherited properties of base class and properties of derived classes
### Generic Identity
#### Sql Server - Generic Identity
Using a Generic Identity without mapping it in DbContext throws an `InvalidOperationException`.
#### Sqlite - Generic Identity
Same behaviour as Sql Server
#### Sql Server - Generic Identity
Successfully generates database for Generic Identity with mapping
#### Sqlite - Generic Identity
Successfully generates database for Generic Identity with mapping
### Identity Strategy
#### Sql Server
throws InvalidOperationException if primary key is not defined or there is no property with 'id' in name
#### Sql Server
generates an id as a column. It also has a Primary Key constraint (id can't be null and has to be unique per record) in table when primary key is defined of right type
#### Sql Server
doesn't need a pk if data annotation says so ([Keyless]). You cannot use CRUD on this table.
#### Sqlite
generates an id as a column. It also has a Primary Key constraint (id can't be null and has to be unique per record) in table when primary key is defined of right type
#### Sqlite
only needs an Id for entities, not for valuetypes inside an entity.
### Required Attributes
#### Sql Server
Generates required properties that will be created even if they are null or empty.
#### Sqlite
Generates required properties that will be created even if they are null or empty.
### Table Per Hierarchy
#### Sql Server
has tph with inherited properties of base class and properties of derived classes.
#### Sqlite
has tph with inherited properties of base class and properties of derived classes.
### Unique Constraints
#### Sql Server
Has unique index on Name.
#### Sqlite
Has combined unique index on Name and Id with a descending order on Id.
### Class Nullability
#### Sql Server
`NullThing` Generates `NULL`.
`SomeThing` Generates `NOT NULL`.
`SomeThingId` adds `ON DELETE CASCADE` to the foreign key definition
`NullThing` does not add `ON DELETE CASCADE` to the foreign key definition
#### Sq Lite
`NullThing` Generates `NULL`.
`SomeThing` Generates `NOT NULL`.
`SomeThingId` adds `ON DELETE CASCADE` to the foreign key definition
`NullThing` does not add `ON DELETE CASCADE` to the foreign key definition
### Int Nullability
#### Sql Server
`int` Generates `int NOT NULL`.
`int?` Generates `int NULL`.
#### Sqlite
`int` Generates `INTEGER NOT NULL`.
`int?` Generates `INTEGER NULL`
### String Nullability
#### Sql Server
`string` Generates `NOT NULL`.
`string?` Generates `NULL`.
#### Sqlite
`string` Generates `NOT NULL`.
`string?` Generates `NULL`.
### Bi Directional One To Many
Because the entity used in the `DbSet` has a collection of another entity type, the latter are mapped as well.
#### Sql Server - bidirectional
EF infers and includes related entities in bidirectional relationship in the schema even when only one side is explicitly registered in the `DbContext`.
#### Sql Server - bidirectional - reversed
EF infers and includes related entities in bidirectional relationship in the schema even when only one side is explicitly registered in the `DbContext`, but reversed this time. CREATE TABLE switches to different DBSets to create tables from.
#### Sqlite - bidirectional
Same behavior as SqlServer with bidirectional relationship, relationship discovery pulls in the `Blog` entity despite only registering `Post`
#### Uni Directional One To Many With One Db Set
Because the entity used in the `DbSet` has a collection of another entity type, the latter are mapped as well.  
EF infers and includes related entities in the schema even when only one side is explicitly registered in the `DbContext`.  

When using the following simple model of *one* `Blog` containing *many* `Posts`: 
```csharp
public class Blog
{
    public int Id { get; set; }
}
```
```csharp
public class Post
{
    public int Id { get; set; }
    public Blog Blog { get; set; } = default!;
}
```
And then adding a `DbSet<Blog>` to the `DbContext` EF generates the following ddl for Sql Server:  

**Blog:**
```csharp
    [
        "CREATE TABLE [Blog] (",
        "    [Id] int NOT NULL IDENTITY,",
        "    CONSTRAINT [PK_Blog] PRIMARY KEY ([Id])",
        ");"
    ];
```
**Post:**
```csharp
    [
        "CREATE TABLE [Posts] (",
        "    [Id] int NOT NULL IDENTITY,",
        "    [BlogId] int NOT NULL,",
        "    CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),",
        "    CONSTRAINT [FK_Posts_Blog_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blog] ([Id]) ON DELETE CASCADE",
        ");"
    ];
```
**Index:**
```csharp
 "CREATE INDEX [IX_Posts_BlogId] ON [Posts] ([BlogId]);";
```
*Note:* No other mappings were added.
