# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["beautysalon.csproj", "beautysalon/"]
RUN dotnet restore "beautysalon/beautysalon.csproj"
COPY . .
WORKDIR "/src/beautysalon"
RUN dotnet build "beautysalon.csproj" -c Release -o /app/build

# Publish the app to /app/publish
FROM build AS publish
RUN dotnet publish "beautysalon.csproj" -c Release -o /app/publish

# Set environment variable for Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Final image with the published app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "beautysalon.dll"]
