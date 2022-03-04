# VanillaDb

An opinionated C# database interface generator that outperforms any reflection-based ORM.  Takes table sql as input and outputs stored-procedures for CRUD operations and get procedures based on available indexes.

# How to use

Build the VanillaDb solution to create a .NET Core commandline application.

## Execution
```code
VanillaDb.exe [table.sql|directory] [outSqlFolder] [outCodeDir] [namespace] [company]
```

The following command would generate interfaces for all table definitions in folder ./foo/, write the stored procedures to ./procs/, write the classes to ./code/ with the namespace foo.bar and include Foobar Inc. in the file headers.
```
VanillaDb.exe ./foo/ ./procs/ ./code/ foo.bar "Foobar Inc."
```
