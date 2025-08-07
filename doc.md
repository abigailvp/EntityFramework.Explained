## Default String Length
### Sql Server
Generates `nvarchar(max)`.
### Sqlite
Generates `TEXT`.
## String Nullability
### Sql Server
`string` Generates `NOT NULL`.
`string?` Generates `NULL`.
### Sqlite
`string` Generates `NOT NULL`.
`string?` Generates `NULL`.
## Uni Directional One To Many With One Db Set
Because the entity used in the `DbSet` has a collection of another entity type, the latter are mapped as well.
### Sql Server
EF infers and includes related entities in the schema even when only one side is explicitly registered in the `DbContext`.
### Sqlite
Same behavior as SqlServer, relationship discovery pulls in the `Blog` entity despite only registering `Post`
