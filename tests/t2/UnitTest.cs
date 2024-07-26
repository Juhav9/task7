using AngleSharp.Dom;
using BlazorPeople.Pages;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PeopleApi;
using PeopleLib;
using Savonia.xUnit.Helpers;
using Savonia.xUnit.Helpers.Infrastructure;
using System;
using System.Linq;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "People.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();
    }

    [Theory]
    [JsonFileData("testdata.json", "get", typeof(int), typeof(Tuple<Person, Address[], ContactInfo[]>))]
    public void Checkpoint02_DetailsAndAddresses(int data, Tuple<Person, Address[], ContactInfo[]> expected)
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);

        // Act
        var cut = ctx.RenderComponent<PersonDetails>(parameters => parameters.Add(p => p.Id, data));
        cut.WaitForState(() => cut.FindAll("table").Count.Equals(1));
        var tableElm = cut.Find("table");
        var markup = cut.Markup;
        var linkHrefs = cut.FindAll("a").Select(l => l.GetAttribute("href"));
        var rows = cut.FindAll("table tr");
        
        // Assert
        Assert.NotNull(tableElm);
        Assert.Contains($"/person/edit/{expected.Item1.Id}", linkHrefs);
        Assert.Contains(expected.Item1.FirstName, markup);
        Assert.Contains(expected.Item1.LastName, markup);
        Assert.Contains(expected.Item1.Title, markup);
        Assert.NotEmpty(rows);
        Assert.Equal(expected.Item2.Length + 1, rows.Count());
        var rowsContent = string.Join("\n", rows.Select(r => r.Html()));
        foreach (var item in expected.Item2)
        {
            Assert.Contains(item.Id.ToString(), rowsContent);
            Assert.Contains(item.StreetAddress, rowsContent);
            Assert.Contains(item.PostalNumber.ToString(), rowsContent);
            Assert.Contains(item.PostalAddress, rowsContent);
        }        
        foreach (var item in expected.Item3)
        {
            Assert.Contains(item.Info!, rowsContent);
        }        
    }

    [Theory]
    [JsonFileData("testdata.json", "get2", typeof(int), typeof(Tuple<Person, string>))]
    public void Checkpoint02_DetailsAndNoAddresses(int data, Tuple<Person, string> expected)
    {
        // Arrange
        // Must create individual contexts for each test run
        var ctx = new TestContext();
        ctx.Services.AddSingleton(_client);

        // Act
        var cut = ctx.RenderComponent<PersonDetails>(parameters => parameters.Add(p => p.Id, data));
        cut.WaitForState(() => cut.FindAll("#personcontent").Count.Equals(1));
        var table = cut.FindAll("table");
        var markup = cut.Markup;

        // Assert
        Assert.Contains(expected.Item1.FirstName, markup);
        Assert.Contains(expected.Item1.LastName, markup);
        Assert.Contains(expected.Item1.Title, markup);
        Assert.Contains(expected.Item2, markup);
        Assert.Equal(0, table.Count);
    }
}