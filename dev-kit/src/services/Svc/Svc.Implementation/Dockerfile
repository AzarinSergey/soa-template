#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/services/Svc/Svc.Implementation/Svc.Implementation.csproj", "src/services/Svc/Svc.Implementation/"]
RUN dotnet restore "src/services/Svc/Svc.Implementation/Svc.Implementation.csproj"
COPY . .
WORKDIR "src/src/services/Svc/Svc.Implementation"
RUN dotnet build "Svc.Implementation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Svc.Implementation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL service.name=svc
LABEL service.version=0.0.1

ENTRYPOINT ["dotnet", "Svc.Implementation.dll"]
