# Project: AI notes app

This application is used to manage notes for given user.

## Architecture

Application consists of two projects. Api and ui. Each defines a separate Dockerfile. At the root level there's docker-compose which helps out with starting the solution.

There should be Aspire project.

### API

#### Conventions

API is based on dotnet 10, so it should use PascalCase as it is defined in dotnet.
Project should be called `NotesApi`

#### File organization

All files related to the API should exist in src/api/ folder.
Vertical Slice Architecture should be used here, each feature should be isolated and exist in separate folder under src/api/Features/. DbContext can be kept under src/api/infrastructure. There's slnx file so this can be easily opened in ide.

#### Libraries

- EF Core - database access
- Npgsql.entityframeworkcore.postgresql - database
- EF Core design - db migrations
- Serilog - logging

EF Core is used to access the database. DbContext can be injected directly to the feature. Use minimal endpoints. Serilog is used for logs. TUnit is used for integration and unit tests. It should use Vertical Slice Architecture/

#### Commands

- Build: `dotnet build`
- Test: `dotnet test`
- Run: `dotnet run`

### UI

UI is written in react vite with typescript.

#### Conventions

As UI project is based on react with Vite and Typescript.
Project is called notes-ui. Default guidelines are followed. Eslint is used to make sure that project doesn't contain issues. Each component is isolated in components folder. Tests are written in jest. Pnpm is used to install packages.

#### Libraries

- React and ESlint

#### Commands

- Run: `pnpm dev`
- Build / publish: `pnpm build`

## Definition of done

A feature is complete when:

1. The specified behavior works correctly across all described scenarios
2. Edge cases identified in the specification are handled
3. A corresponding unit test exists and passes
4. No linting errors are introduced
5. The feature works correctly at desktop and mobile viewport widths
