FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/Domain/Copasogo.Domain.csproj src/Domain/
COPY src/Application/Copasogo.Application.csproj src/Application/
COPY src/Infra.Data/Copasogo.Infra.Data.csproj src/Infra.Data/
COPY src/WebApi/Copasogo.WebApi.csproj src/WebApi/

RUN dotnet restore src/WebApi/Copasogo.WebApi.csproj

COPY src/Domain/ src/Domain/
COPY src/Application/ src/Application/
COPY src/Infra.Data/ src/Infra.Data/
COPY src/WebApi/ src/WebApi/

RUN dotnet publish src/WebApi/Copasogo.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Copasogo.WebApi.dll"]
