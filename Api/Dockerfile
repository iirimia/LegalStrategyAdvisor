FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5199

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Api/LegalStrategyAdvisor.Api.csproj", "Api/"]
RUN dotnet restore "Api/LegalStrategyAdvisor.Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "LegalStrategyAdvisor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LegalStrategyAdvisor.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LegalStrategyAdvisor.Api.dll"]