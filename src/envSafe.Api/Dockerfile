FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/envSafe.Api/envSafe.Api.csproj", "src/envSafe.Api/"]
RUN dotnet restore "src/envSafe.Api/envSafe.Api.csproj"

COPY . .
WORKDIR "/src/src/envSafe.Api"
RUN dotnet publish "envSafe.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "envSafe.Api.dll"]
