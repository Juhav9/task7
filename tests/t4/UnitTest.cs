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
using PeopleLib;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
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
    private string location = "";

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "People.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();

        string connectionstring = "Data Source=People.db";

        var optionsBuilder = new DbContextOptionsBuilder<PeopleContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new PeopleContext(optionsBuilder.Options);
    }

    [Fact]
    public async Task Checkpoint04_Create()
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);
        System.Random r = new System.Random();
        var address = new Address
        {
            StreetAddress = "Tester Lane 2024",
            PostalNumber = r.Next(),
            PostalAddress = "CI/CD"
        };
        var navMan = ctx.Services.GetRequiredService<FakeNavigationManager>();
        navMan.LocationChanged += NavMan_LocationChanged;
        
        // Act
        var cut = ctx.RenderComponent<CreateAddress>();
        cut.WaitForState(() => cut.HasComponent<EditForm>());
        var markup = cut.Markup;
        var inputs = cut.FindAll("input");
        var submit = cut.Find("button#addaddress");

        // Assert
        Assert.NotNull(inputs);
        Assert.NotEmpty(inputs);
        Assert.Equal(3, inputs.Count);
        Assert.Equal("Add new address item", submit.TextContent);
        Assert.True(cut.HasComponent<InputText>());
        Assert.True(cut.HasComponent<InputNumber<int>>());

        // Act 2
        cut.FindAll("input")[0].Change(address.StreetAddress);
        cut.FindAll("input")[1].Change(address.PostalAddress);
        cut.FindAll("input")[2].Change(address.PostalNumber);

        await submit.ClickAsync(new MouseEventArgs());

        var dbAddress = await _db.Addresses.OrderBy(a => a.Id).LastOrDefaultAsync();

        // Assert 2
        Assert.NotNull(dbAddress);
        Assert.Equal(address.StreetAddress, dbAddress.StreetAddress);
        Assert.Equal(address.PostalNumber, dbAddress.PostalNumber);
        Assert.Equal(address.PostalAddress, dbAddress.PostalAddress);

        // Assert 3
        Assert.Equal(location, navMan.Uri);
    }

    [Fact]
    public void Checkpoint04_Link()
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);
        var expected = "address/create";

        // Act
        var cut = ctx.RenderComponent<NavMenu>();
        var navLinks = cut.FindComponents<NavLink>();

        // Assert
        Assert.NotEmpty(navLinks);

        var link = navLinks.FirstOrDefault(n => n.Markup.Contains(expected, System.StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(link);
    }

    private void NavMan_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        location = e.Location;
    }
}