using BlazorPeople.Pages;
using BlazorPeople.Shared;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeopleApi;
using PeopleApi.Data;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly PeopleContext _db;
    private readonly HttpClient _client;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "People.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();

        string connectionstring = "Data Source=People.db";

        var optionsBuilder = new DbContextOptionsBuilder<PeopleContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new PeopleContext(optionsBuilder.Options);
    }

    [Theory]
    [JsonFileData("testdata.json", "join", typeof(Tuple<int, int>), typeof(Tuple<int, int, string>))]
    public async Task Checkpoint05(Tuple<int, int> data, Tuple<int, int, string> expected)
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);
        var navMan = ctx.Services.GetRequiredService<FakeNavigationManager>();

        System.Random r = new System.Random();
        var info = $"test info\n{r.Next()}";

        // Act
        var cut = ctx.RenderComponent<JoinAddressToPerson>();
        cut.WaitForState(() => cut.HasComponent<EditForm>());
        var markup = cut.Markup;
        var selects = cut.FindAll("select");
        var submit = cut.Find("button#joindata");
        var forAddress = cut.Find("#for-address");
        var forAddressItems = cut.FindAll("#for-address option");
        var forPerson = cut.Find("#for-person");
        var forPersonItems = cut.FindAll("#for-person option");
        var forInfo = cut.Find("textarea");

        // Assert
        Assert.NotNull(selects);
        Assert.Equal(2, selects.Count);
        Assert.NotEmpty(forAddressItems);
        Assert.NotEmpty(forPersonItems);
        Assert.Equal(expected.Item1, forPersonItems.Count);
        Assert.Equal(expected.Item2, forAddressItems.Count);
        Assert.Equal("Join address to person", submit.TextContent);
        Assert.True(cut.HasComponent<InputSelect<int>>());
        Assert.True(cut.HasComponent<InputTextArea>());
        

        // Act 2
        forPerson.Change(data.Item1);
        forAddress.Change(data.Item2);
        forInfo.Change(info);

        await submit.ClickAsync(new MouseEventArgs());

        var dbContact = await _db.ContactInfos.FirstOrDefaultAsync(c => c.PersonId == data.Item1 && c.AddressId == data.Item2);

        // Assert 2
        Assert.NotNull(dbContact);
        Assert.Equal(info, dbContact.Info);

        // Assert 3
        Assert.Contains($"{expected.Item3}", navMan.Uri);
    }

    [Fact]
    public void Checkpoint05_Link()
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);
        var expected = "join";

        // Act
        var cut = ctx.RenderComponent<NavMenu>();
        var navLinks = cut.FindComponents<NavLink>();

        // Assert
        Assert.NotEmpty(navLinks);

        var link = navLinks.FirstOrDefault(n => n.Markup.Contains(expected, System.StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(link);
    }
}