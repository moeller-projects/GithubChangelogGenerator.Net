FROM mcr.microsoft.com/dotnet/core/runtime:3.1.11-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/GithubChangelogGenerator.Net/GithubChangelogGenerator.Net.csproj", "src/GithubChangelogGenerator.Net/"]
RUN dotnet restore "src/GithubChangelogGenerator.Net/GithubChangelogGenerator.Net.csproj"
COPY . .
WORKDIR "/src/src/GithubChangelogGenerator.Net"
RUN dotnet build "GithubChangelogGenerator.Net.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GithubChangelogGenerator.Net.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "github-changelog-generator.dll"]