# ✅ Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ✅ SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["ECommerceApi.csproj", "./"]
RUN dotnet restore "ECommerceApi.csproj"

# Copy the remaining source code and build the app
COPY . .
RUN dotnet publish "ECommerceApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ✅ Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# ✅ Entry point
ENTRYPOINT ["dotnet", "ECommerceApi.dll"]
