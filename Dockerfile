# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["FindTheBug.sln", "./"]
COPY ["src/FindTheBug.Domain/FindTheBug.Domain.csproj", "src/FindTheBug.Domain/"]
COPY ["src/FindTheBug.Application/FindTheBug.Application.csproj", "src/FindTheBug.Application/"]
COPY ["src/FindTheBug.Infrastructure/FindTheBug.Infrastructure.csproj", "src/FindTheBug.Infrastructure/"]
COPY ["src/FindTheBug.WebAPI/FindTheBug.WebAPI.csproj", "src/FindTheBug.WebAPI/"]

# Restore dependencies
RUN dotnet restore "src/FindTheBug.WebAPI/FindTheBug.WebAPI.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/FindTheBug.WebAPI"
RUN dotnet build "FindTheBug.WebAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FindTheBug.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FindTheBug.WebAPI.dll"]
