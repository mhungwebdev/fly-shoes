#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FlyShoes.API/FlyShoes.API.csproj", "FlyShoes.API/"]
COPY ["FlyShoes.BL/FlyShoes.BL.csproj", "FlyShoes.BL/"]
COPY ["FlyShoes.Common/FlyShoes.Common.csproj", "FlyShoes.Common/"]
COPY ["FlyShoes.Repository/FlyShoes.DAL.csproj", "FlyShoes.Repository/"]
RUN dotnet restore "FlyShoes.API/FlyShoes.API.csproj"
COPY . .
WORKDIR "/src/FlyShoes.API"
RUN dotnet build "FlyShoes.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlyShoes.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlyShoes.API.dll"]