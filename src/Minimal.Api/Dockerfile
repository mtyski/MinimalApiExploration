#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Minimal.Api/Minimal.Api.csproj", "Minimal.Api/"]
COPY ["Minimal.Application/Minimal.Application.csproj", "Minimal.Application/"]
COPY ["Minimal.Db/Minimal.Db.csproj", "Minimal.Db/"]
COPY ["Minimal.Model/Minimal.Model.csproj", "Minimal.Model/"]
COPY ["Minimal.Redis/Minimal.Redis.csproj", "Minimal.Redis/"]
RUN dotnet restore "Minimal.Api/Minimal.Api.csproj"
COPY . .
WORKDIR "/src/Minimal.Api"
RUN dotnet build "Minimal.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Minimal.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Minimal.Api.dll"]
