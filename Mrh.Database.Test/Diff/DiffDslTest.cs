using System;
using System.Collections.Generic;
using FakeItEasy;
using Mrh.Database.Diff;
using Mrh.DataStructures.Graph;
using NUnit.Framework;

namespace Mrh.Database.Test.Diff
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
        }

        public class OneToOneNodeDb : AbstractDatabaseRecord<int>
        {
            public int Field3 { get; set; }
            
            public string Field4 { get; set; }

            public override int Id { get; set; }
        }

        [Test]
        public void TestCreate()
        {
            var dsl = CreateDiffDsl();
            var result = dsl.Build();
            var length = result.Item2;
            var root = result.Item1;
            Assert.AreEqual(2, length);
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
                    Field4 = "4"
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
                    Field4 = "3"
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
            Assert.AreEqual(2, length);
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
                    Field4 = "4"
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
                    Field4 = "3"
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
                            d => d.Field4));
        }
    }
}