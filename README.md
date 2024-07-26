[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/JKZMHVEy)
# Task 07: Blazor WASM

<img alt="points bar" align="right" height="36" src="../../blob/badges/.github/badges/points-bar.svg" />

![GitHub Classroom Workflow](../../workflows/GitHub%20Classroom%20Workflow/badge.svg)

***

## Student info

> Write your name, your estimation of how many points you will get, and an estimate of how long it took to make the answer

- Student name: 
- Estimated points: 
- Estimated time (hours): 

***

## Purpose of this task

The purposes of this task are:

- to learn to make a Blazor WebAssembly app
- to learn to use external web api in a Blazor app
- to learn about basic controls in Blazor

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before start coding.

1. [ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-6.0)
2. [Build a Blazor todo list app](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app?view=aspnetcore-6.0&pivots=webassembly)
3. [Call a web API from ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-6.0&pivots=webassembly)
4. [ASP.NET Core Blazor routing and navigation](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-6.0)
5. [ASP.NET Core Blazor forms and input components](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-6.0)

## The Task

Create a Blazor WebAssembly (Blazor WASM) app to handle person and address data. Create UI for all people from the API and UIs for adding new persons, addresses and joining an address to a person. The BlazorWASM template (BlazorPeople), API and data model are given. The API uses Sqlite as database but the data model is simplified with no database relations with different types of data. 

> **Note!** The API is simplified for this task's purpose and it does not represent the way how a web API should be properly made for production apps. 

Reference the PeopleLib class library project in the Blazor app so you have access to the model classes.

Use the given API endpoints from the Blazor WASM app. The Blazor app handles joining the data so that proper data is shown (i.e. only addresses linked to the person is shown for the person etc.). **Do NOT make any modifications to the web API**. Use async calls to the API and handle them properly in the client app.

> It is the client app's job (i.e. the blazor app) to request enough data from the API that it can do the required tasks.

> **Note!** `HttpClient` is the only configured external dependency that the Blazor app pages can use. Other usable default dependencies are `NavigationManager` and `ILogger<T>`.

- `src/BlazorPeople` is the BlazorWASM app. This needs to be edited!
- `src/PeopleApi` is the web API. Note that the API is very simplified and its usage requires data handling in the client app.
- `src/PeopleLib` is a class library containing the data models used by both the Blazor app and the web API.

> **Note!** Remember to start the PeopleApi! Instructions in src/README.md file.

### Evaluation points

The URLs in the following evaluation points are URLs in the client app. The API URLs must be reasoned from the intended functionality.

1. The app root (Index.razor) (/) loads people from api and displays their first name, last name and title in a [Card](https://getbootstrap.com/docs/4.0/components/card/) (i.e. a `div` element styled as a card). The card element's title contains the `person's first name` and `last name` (in this order). The card contains also the `person's title` and a link (`a` element) styled as a bootstrap button to the selected person's details page (/person/details/[id]), where the [id] is the selected person's id. The card elements are rendered in a `div` element with id property value `allpeople` only after the initial data fetch is completed. The page must handle fetching the required data from the API.
2. A page (PersonDetails.razor) to show person details (/person/details/[id]). Shows all of a person's info and all addresses (`Address` class data) related to the selected person with possible contact info's info text (`ContactInfo` class data). The address data is shown in a `table` element with columns for each of the readable properties in `Address` class and a column for the possible info text. The table element has headers. The table element is rendered only if the selected person has addresses info. If the person does not have any addresses then text `Selected person does not have any addresses` is rendered in `p` element. The details page contains also a link to edit page (/person/edit/[id]). The contents of the page can be rendered only after the initial requests to the API has completed. The content is rendered in a `div` element with id property value `personcontent`. The page must handle fetching the required data from the API.
3. A page (PersonEdit.razor) (/person/edit/[id]) to edit the selected person's textual (`string`) data. Use `EditForm` control to edit the data and use HTTP PUT to submit the changes to the API. Add only a single submit button to the page (use `button` element with id property value `save`). The submit button has text value `Save changes` shown to the user. Clicking the submit button causes the data to be submitted to the API. When the edit succeeds then the user is redirected to the details page for the selected person (i.e. the details page is shown). The page must handle fetching the required data from the API and render the edit form only after the data is received from the API.
4. A page (CreateAddress.razor) to create new address data (/address/create). The page contains a form to input data for a new address entity (`Address` class). Use `EditForm` control. The `Id` property of Address class must not have a field in the form. Use `InputText` or `InputNumber` components for the rest of the Address class' properties. Add the edit fields in the same order as the properties are listed in the Address class. Add a submit button (use `button` element with id property value `addaddress`) with textual value `Add new address item` shown to the user that will submit the properly inputted data when clicked. The data is submitted via HTTP POST request to the API. When the add succeeds the user is redirected to the root of the app (/). The app's default navigation bar must have a `NavLink` component to the CreateAddress page.
5. A page (JoinAddressToPerson.razor) for joining an address to a person (/join). In the page there are selections for an address and for the person and an additional textual field for info text. Use `InputSelect` components for selections and `InputTextArea` for the info text. Add an `id` attribute to the select fields so that the field for selecting address has an `id="for-address"` and the field for selecting person has an `id="for-person"`. Add a submit button (use `button` element with id property value `joindata`) with textual value `Join address to person` shown to the user that will submit the properly inputted data when clicked. The controls are in `EditForm` control. The data is saved via POST request to the API. When the joining succeeds the user is redirected to the selected person's details page. The page must handle fetching the required data from the API. The app's default navigation bar must have a `NavLink` component to the JoinPersonToAddress page.

> **Note!** Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or does not work there is no "it kind of works" step in between. Be sure to test your work. All working evaluation points are added to the task total and will count toward the course total. The task is worth five (5) points. Each evaluation point is checked individually and each will provide one (1) point so there are five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on the main branch. So remember to `git commit` and `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in the folder named .github.

> **DO NOT modify the contents of .github or tests folders.**
