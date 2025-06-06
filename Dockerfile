# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Web/Web.csproj", "Web/"]
RUN dotnet restore "Web/Web.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish
RUN dotnet publish "Web/Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy from build stage
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
