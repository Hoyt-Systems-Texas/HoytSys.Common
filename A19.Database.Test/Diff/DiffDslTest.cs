using System;
using System.Collections.Generic;
using A19.Database.Diff;
using A19.DataStructures.Graph;
using FakeItEasy;
using NUnit.Framework;

namespace A19.Database.Test.Diff
{
    [TestFixture]
    public class DiffDslTest
    {
        public class RootNode
        {
            public int Id { get; set; }
            public int Field1 { get; set; }
            public int Field2 { get; set; }
            
            public OneToOneNode One { get; set; }
        }

        public class RootNodeDb : AbstractDatabaseRecord<int>
        {
            public int Field1 { get; set; }

            public int Field2 { get; set; }
            public override int Id { get; set; }
            
            public OneToOneNodeDb One { get; set; }
        }

        public class OneToOneNode
        {
            public int Field3 { get; set; }
            public string Field4 { get; set; }
            
            public List<ManyToManyNode> Field6 { get; set; }
        }

        public class OneToOneNodeDb : AbstractDatabaseRecord<int>
        {
            public OneToOneNodeDb()
            {
                this.Field6 = new List<ManyToManyNodeDb>(0);
            }
            public int Field3 { get; set; }
            
            public string Field4 { get; set; }

            public override int Id { get; set; }
            
            public List<ManyToManyNodeDb> Field6 { get; set; }
        }

        public class ManyToManyNode
        {
            public int Id { get; set; }
            
            public string Field5 { get; set; }
        }

        public class ManyToManyNodeDb : AbstractDatabaseRecord<int>
        {
            public override int Id { get; set; }
            
            public string Field5 { get; set; }
        }

        [Test]
        public void TestCreate()
        {
            var dsl = CreateDiffDsl();
            var result = dsl.Build();
            var length = result.Item2;
            var root = result.Item1;
            Assert.AreEqual(3, length);
            var graph = new UniformMatrixGraph(length);
            root.BuildDependencyGraph(graph);
            var deps = graph.FindDependencies();
            Assert.IsNotEmpty(deps);

            var rootValue = new RootNode
            {
                Field1 = 1,
                Field2 = 2,
                Id = 3,
                One = new OneToOneNode
                {
                    Field3 = 3,
                    Field4 = "4",
                    Field6 =  new List<ManyToManyNode>(0)
                }
            };
            var rootValueDb = new RootNodeDb
            {
                Field1 = 1,
                Field2 = 3,
                Id = 3,
                One = new OneToOneNodeDb
                {
                    Field3 = 4,
                    Field4 = "3",
                    Field6 = new List<ManyToManyNodeDb>(0)
                }
            };
            var dict = new Dictionary<int, IUpdateRecords<Guid>>(2);
            root.Update(
                rootValue,
                rootValueDb,
                dict);
            Assert.IsNotEmpty(dict);
        }

        [Test]
        public void TestNotImmutable()
        {
            
            var dsl = CreateDiffDsl();
            var result = dsl.Build();
            var length = result.Item2;
            var root = result.Item1;
            Assert.AreEqual(3, length);
            var graph = new UniformMatrixGraph(length);
            root.BuildDependencyGraph(graph);
            var deps = graph.FindDependencies();
            Assert.IsNotEmpty(deps);

            var rootValue = new RootNode
            {
                Field1 = 1,
                Field2 = 2,
                Id = 3,
                One = new OneToOneNode
                {
                    Field3 = 3,
                    Field4 = "4",
                    Field6 = new List<ManyToManyNode>
                    {
                        new ManyToManyNode
                        {
                            Field5 = "5_1"
                        },
                        new ManyToManyNode
                        {
                            Field5 = "5_2"
                        },
                        new ManyToManyNode
                        {
                            Id = 1,
                            Field5 = "5_3"
                        },
                        new ManyToManyNode
                        {
                            Field5 = "5_4",
                            Id = 3
                        },
                        new ManyToManyNode
                        {
                            Field5 = "5_7",
                            Id = 4
                        }
                    }
                }
            };
            var rootValueDb = new RootNodeDb
            {
                Field1 = 1,
                Field2 = 2,
                Id = 3,
                One = new OneToOneNodeDb
                {
                    Field3 = 4,
                    Field4 = "3",
                    Field6 = new List<ManyToManyNodeDb>
                    {
                        new ManyToManyNodeDb
                        {
                            Id = 1,
                            Field5 = "5_3"
                        },
                        new ManyToManyNodeDb
                        {
                            Id = 2
                        },
                        new ManyToManyNodeDb
                        {
                            Id = 3,
                            Field5 = "5_5"
                        },
                        new ManyToManyNodeDb
                        {
                            Field5 = "5_7",
                            Id = 4
                        }
                    }
                }
            };
            var dict = new Dictionary<int, IUpdateRecords<Guid>>(2);
            root.Update(
                rootValue,
                rootValueDb,
                dict);
            Assert.IsNotEmpty(dict);
        }

        private DiffDsl<RootNode, RootNodeDb, int, Guid> CreateDiffDsl()
        {
            return new DiffDsl<RootNode, RootNodeDb, int, Guid>(
                    A.Fake<IDiffRepository<Guid, int, RootNodeDb>>(),
                    false)
                .Add(
                    n => n.Field1,
                    d => d.Field1)
                .Add(
                    n => n.Field2,
                    d => d.Field2)
                .AddOne(
                    n => n.One,
                    d => d.One,
                    new DiffDsl<OneToOneNode,OneToOneNodeDb,int,Guid>(
                        A.Fake<IDiffRepository<Guid, int, OneToOneNodeDb>>(),
                        true)
                        .Add(
                            n => n.Field3,
                            d => d.Field3)
                        .Add(
                            n => n.Field4,
                            d => d.Field4)
                        .AddMany(
                            n => n.Id,
                            n => n.Field6,
                            d => d.Field6,
                            (c, p) => p.Field6.Add(c),
                            new DiffDsl<ManyToManyNode, ManyToManyNodeDb, int, Guid>(
                                A.Fake<IDiffRepository<Guid, int, ManyToManyNodeDb>>(),
                                true)
                                .Add(
                                    n => n.Id,
                                    d => d.Id)
                                .Add(
                                    n => n.Field5,
                                    d => d.Field5)));
        }
    }
}