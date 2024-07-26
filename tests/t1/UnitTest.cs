using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeopleApi;
using PeopleApi.Data;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>, IClassFixture<TestContext>
{
    private readonly PeopleContext _db;
    private readonly TestContext _ctx;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture, TestContext ctx) : base(new string[] { "People.db" }, testOutputHelper)
    {
        var client = fixture.CreateClient();
        _ctx = ctx;
        _ctx.Services.AddSingleton(client);

        string connectionstring = "Data Source=People.db";

        var optionsBuilder = new DbContextOptionsBuilder<PeopleContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new PeopleContext(optionsBuilder.Options);
    }

    [Theory]
    [JsonFileData("testdata.json", "get", typeof(Tuple<string, string>), typeof(int))]
    public async Task Checkpoint01_Index(Tuple<string, string> data, int expected)
    {
        // verify that all people are listed on the index page as instructed

        // Arrange

        // Act
        var cut = _ctx.RenderComponent<BlazorPeople.Pages.Index>();
        cut.WaitForState(() => cut.FindAll(data.Item1).Count.Equals(expected));
        var content = cut.Find("div#allpeople");
        var cards = cut.FindAll(data.Item1);
        var names = cut.FindAll(data.Item2);
        var markup = cut.Markup;
        var linkHrefs = cut.FindAll("a.btn").Select(l => l.GetAttribute("href"));
        var people = await _db.People.ToListAsync();

        // Assert
        Assert.NotNull(cards);
        Assert.NotEmpty(cards);
        Assert.NotNull(names);
        Assert.NotEmpty(names);
        var namesContent = names.Select(t => t.InnerHtml.Condense()).ToArray();
        foreach (var item in people)
        {
            Assert.Contains(item.FirstName, markup);
            Assert.Contains(item.LastName, markup);
            Assert.Contains(item.Title, markup);
            Assert.Contains($"/person/details/{item.Id}", linkHrefs);
            Assert.Contains($"{item.FirstName} {item.LastName}".Condense(), namesContent);
        }
    }
}