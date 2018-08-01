using System;
using System.Linq;
using System.Collections.Generic;

namespace Cosmos.SqlBuilder
{
    class Program
    {
        static List<Func<(string, string)>> tests = new List<Func<(string, string)>>() 
        {
            Test1,
            Test2,
            Test3,
            Test4,
            Test5,
            Test6,
            Test7,
            Test8,
            Test9,
            Test10,
            Test11,
            Test12,
            Test13,
            Test14,
            Test15,
            Test16
        };

        static void Main(string[] args)
        {
            foreach (var test in tests)
            {
                string actual, expected;
                (actual, expected) = test();

                if(actual != expected)
                {
                    Console.WriteLine($"Error at {test.Method.Name} !!!: \n Actual:   {actual}\n Expected: {expected}");
                }
            }
            Console.ReadKey();
        }

        static (string, string) Test1()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Text)
                .Select(x => x.Key)
                .Build();

            return (text, "SELECT c.Text, c.Key FROM c");
        }

        static (string, string) Test2()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA, 
                    y => y.Select(z => z.Key)
                )
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key } as InnerA FROM c");
        }

        static (string, string) Test3()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA, 
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA)
                )
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": c.InnerA.InnerA } as InnerA FROM c");
        }

        static (string, string) Test4()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA, 
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c");
        }


        static (string, string) Test5()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Key == "asd")
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE c.Key = 'asd'");
        }

        static (string, string) Test6()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Key == "asd" && x.InnerA.Key == "ASD")
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE (c.Key = 'asd') AND (c.InnerA.Key = 'ASD')");
        }

        static (string, string) Test7()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Key == "asd" || x.InnerA.Key == "ASD")
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE (c.Key = 'asd') OR (c.InnerA.Key = 'ASD')");
        }

        static (string, string) Test8()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Text == 7)
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE c.Text = 7");
        }

        static (string, string) Test9()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Text != 7)
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE c.Text != 7");
        }

        static (string, string) Test10()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Select(x => x.InnerA,
                    y => y.Select(z => z.Key)
                    .Select(z => z.InnerA, a => a.Select(z => z.Key))
                )
                .Where(x => x.Text == 7 - 10)
                .Build();

            return (text, "SELECT c.Key, c.Text, { \"Key\": c.InnerA.Key, \"InnerA\": { \"Key\": c.InnerA.InnerA.Key } } as InnerA FROM c WHERE c.Text = -3");
        }

        static (string, string) Test11()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Select(x => x.Text)
                .Where(x => x.BoolVal == false)
                .Build();

            return (text, "SELECT c.Key, c.Text FROM c WHERE c.BoolVal = false");
        }
        
        static (string, string) Test12()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.Key)
                .Where(x => x.BoolVal)
                .OrderByDescending(x => x.Text)
                .Build();

            return (text, "SELECT c.Key FROM c WHERE c.BoolVal ORDER BY c.Text DESC");
        }
        
        static (string, string) Test13()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Top(15)
                .Select(x => x.Key)
                .OrderByAscending(x => x.Text)
                .Build();

            return (text, "SELECT TOP 15 c.Key FROM c ORDER BY c.Text ASC");
        }

        static (string, string) Test14()
        {
            SqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .SelectAll()
                .Where(x => !x.BoolVal)
                .Build();

            return (text, "SELECT * FROM c WHERE NOT c.BoolVal");
        }

        static (string, string) Test15()
        {
            ISqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<A>()
                .Select(x => x.BoolVal)
                .Select(x => x.InnerA, y => y.Select(z => z.Key))
                .Where(x => (x.BoolVal || x.Text == 5) && x.Key == "asd")
                .OrderByAscending(x => x.Key)
                .Build();

            return (text, "SELECT c.BoolVal, { \"Key\": c.InnerA.Key } as InnerA FROM c WHERE ((c.BoolVal) OR (c.Text = 5)) AND (c.Key = 'asd') ORDER BY c.Key ASC");
        }

        class Foo
        {
            public Foo InnerFoo { get; set; }
            public string Bar { get; set; }
            public bool Baz { get; set; }
        }

        static (string, string) Test16()
        {
            ISqlBuilder builder = new SqlBuilder();

            var text = builder
                .From<Foo>()
                .Select(x => x.Bar)
                .Select(x => x.InnerFoo, y => y.Select(z => z.Bar))
                .Where(x => x.Baz && x.Bar == "FOO")
                .OrderByAscending(x => x.Bar)
                .Build();

            return (text, "");
        }
    }
}
