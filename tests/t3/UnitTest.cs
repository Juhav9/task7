using BlazorPeople.Pages;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeopleApi;
using PeopleApi.Data;
using PeopleLib;
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
    [JsonFileData("testdata.json", "put", typeof(int), typeof(Tuple<int, Person>))]
    public async Task Checkpoint03(int data, Tuple<int, Person> expected)
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client); 
        var navMan = ctx.Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = ctx.RenderComponent<PersonEdit>(parameters => parameters.Add(p => p.Id, data));
        cut.WaitForState(() => cut.HasComponent<EditForm>());
        var markup = cut.Markup;
        var edit = cut.FindComponent<EditForm>();
        var inputs = cut.FindAll("input");
        var inputValues = inputs.Select(i => i.GetAttribute("value"));
        var submit = cut.Find("button#save");

        // Assert
        Assert.NotNull(edit);
        Assert.NotNull(inputs);
        Assert.NotEmpty(inputs);
        Assert.Equal(expected.Item1, inputs.Count);
        Assert.Equal("Save changes", submit.TextContent);
        Assert.Contains(expected.Item2.FirstName, inputValues);
        Assert.Contains(expected.Item2.LastName, inputValues);
        Assert.Contains(expected.Item2.Title, inputValues);

        // Act 2
        var titleInput = inputs.FirstOrDefault(i => i.GetAttribute("value") == expected.Item2.Title);
        Assert.NotNull(titleInput);
        System.Random r = new System.Random();
        string newTitle = $"changed by test - {r.Next()}";
        titleInput.Change(newTitle);
        await submit.ClickAsync(new MouseEventArgs());

        var dbPerson = await _db.People.FindAsync(data);

        // Assert 2
        Assert.NotNull(dbPerson);
        Assert.Equal(newTitle, dbPerson.Title);

        // Assert 3
        Assert.Contains($"/person/details/{expected.Item2.Id}", navMan.Uri);
    }
}