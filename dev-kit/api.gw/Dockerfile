FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Api.Gw/Api.Gw.csproj", "Api.Gw/"]
RUN dotnet restore "Api.Gw/Api.Gw.csproj"
COPY . .
WORKDIR "/src/Api.Gw"
RUN dotnet build "Api.Gw.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.Gw.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL service.name=api.gw
LABEL service.version=0.0.1

ENTRYPOINT ["dotnet", "Api.Gw.dll"]