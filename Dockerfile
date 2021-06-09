FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Stonks.csproj", "./"]
RUN dotnet restore "Stonks.csproj"
COPY . .
WORKDIR "/src/Stonks"
RUN dotnet build "Stonks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stonks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stonks.dll"]
