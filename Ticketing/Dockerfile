FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Ticketing/Ticketing.csproj", "Ticketing/"]
RUN dotnet restore "Ticketing/Ticketing.csproj"
COPY . .
WORKDIR "/src/Ticketing"
RUN dotnet build "Ticketing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ticketing.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ticketing.dll"]
