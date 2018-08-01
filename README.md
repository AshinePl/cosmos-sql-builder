# cosmos-sql-builder

Using:
```C#
ISqlBuilder builder = new SqlBuilder();

var text = builder
            .From<Foo>()
            .Select(x => x.Bar)
            .Select(x => x.InnerFoo, y => y.Select(z => z.Bar))
            .Where(x => x.Baz && x.Bar == "FOO")
            .OrderByAscending(x => x.Bar)
            .Build();


"SELECT c.Bar, { "Bar": c.InnerFoo.Bar } as InnerFoo FROM c WHERE (c.Baz) AND (c.Bar = 'FOO') ORDER BY c.Bar ASC"
```