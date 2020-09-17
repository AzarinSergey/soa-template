#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["src/services/Exs/Exs.Implementation/Exs.Implementation.csproj", "src/services/Exs/Exs.Implementation/"]
RUN dotnet restore "src/services/Exs/Exs.Implementation/Exs.Implementation.csproj"
COPY . .
WORKDIR "/src/src/services/Exs/Exs.Implementation"
RUN dotnet build "Exs.Implementation.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Exs.Implementation.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Exs.Implementation.dll"]
