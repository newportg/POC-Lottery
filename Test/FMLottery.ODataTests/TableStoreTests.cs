using AutoMapper;
using Common.Mappings;
using Models;
using OData.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace OData.Tests
{
    //[TestClass()]
    //public class TableStoreTests
    //{
    //    // GetByDrawNumber
    //    [TestMethod()]
    //    public void GetByDrawNumberTest()
    //    {
    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Select("RowKey eq '2639'");

    //        //Assert.IsTrue(res.Count == 1);
    //        //Assert.IsTrue(res[0].RowKey.Contains("2639"));
    //    }

    //    // ListAll
    //    [TestMethod()]
    //    public void ListAllTest()
    //    {
    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Select("PartitionKey eq 'Thunderball'");

    //        var configuration = new MapperConfiguration(cfg => { });
    //        var result = Mapper.Map<List<TableEntity>>(res.value);

    //        Assert.IsFalse(res.v.Count == 0);

    //    }

    //    // ListUpToDrawNumber
    //    [TestMethod()]
    //    public void ListUpToDrawNumberTest()
    //    {
    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Select("RowKey le '2640'");

    //        //Assert.IsFalse(res.Count == 0);

    //    }

    //    // Insert
    //    [TestMethod()]
    //    public void InsertTest()
    //    {
    //        var ent = new ThunderBallEntity
    //        {
    //            PartitionKey = "Thunderball",
    //            RowKey = "9990",
    //            DrawDate = "2020-11-26",
    //            DrawNumber = "9990",
    //            Ball1 = "1",
    //            Ball2 = "2",
    //            Ball3 = "3",
    //            Ball4 = "4",
    //            Ball5 = "5",
    //            BallSet = "1",
    //            Machine = "1",
    //            Thunderball = "11"
    //        };

    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Insert(ent);

    //       Assert.IsFalse(res == null);

    //    }

    //    // Delete
    //    [TestMethod()]
    //    public void DeleteTest()
    //    {
    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Delete("9990", "Thunderball");

    //        Assert.IsFalse(res == null);
    //    }

    //    // Update
    //    [TestMethod()]
    //    public void UpdateTest()
    //    {
    //        var ent = new ThunderBallEntity
    //        {
    //            PartitionKey = "Thunderball",
    //            RowKey = "9990",
    //            DrawDate = "2020-11-26",
    //            DrawNumber = "CHANGED",
    //            Ball1 = "1",
    //            Ball2 = "2",
    //            Ball3 = "3",
    //            Ball4 = "4",
    //            Ball5 = "5",
    //            BallSet = "1",
    //            Machine = "1",
    //            Thunderball = "11"
    //        };

    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Update(ent);

    //        Assert.IsFalse(res == null);
    //    }

    //    // Update
    //    [TestMethod()]
    //    public void UpsertTest()
    //    {
    //        var ent = new ThunderBallEntity
    //        {
    //            PartitionKey = "Thunderball",
    //            RowKey = "9000",
    //            DrawDate = "2020-11-306",
    //            DrawNumber = "Upsert",
    //            Ball1 = "1",
    //            Ball2 = "2",
    //            Ball3 = "3",
    //            Ball4 = "4",
    //            Ball5 = "5",
    //            BallSet = "1",
    //            Machine = "1",
    //            Thunderball = "11"
    //        };

    //        var c1 = new TableStore("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        var res = c1.Upsert(ent);

    //        Assert.IsFalse(res == null);
    //    }

    //    //--- Helpers
    //    private ServiceProvider Bootstrapper()
    //    {
    //        var services = new ServiceCollection();
    //        AutoMapperProfile.AddMapProfiles(services, "FMLottery");
    //        services.AddODataServices("fmlotterystrvsene", "8S2APGgD48VzRWXmeA0AaoreWCdIDbl6vionhBdCYiQ%3D", "ThunderBall");
    //        services.AddLogging();
    //        return services.BuildServiceProvider();
    //    }

    //}
}