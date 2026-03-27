# ===== BASE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8668                                     
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8668               
ENV PORT=8668

# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["QueueManagement.API/QueueManagement.API.csproj", "QueueManagement.API/"]
COPY ["QueueManagement.Infrastructure/QueueManagement.Infrastructure.csproj", "QueueManagement.Infrastructure/"]
COPY ["QueueManagement.Application/QueueManagement.Application.csproj", "QueueManagement.Application/"]
COPY ["QueueManagement.Domain/QueueManagement.Domain.csproj", "QueueManagement.Domain/"]

RUN dotnet restore "./QueueManagement.API/QueueManagement.API.csproj"

COPY . .
WORKDIR "/src/QueueManagement.API"

RUN dotnet publish "./QueueManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# ===== FINAL =====
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

## 🔥 QUAN TRỌNG CHO RENDER
#ENV ASPNETCORE_URLS=http://+:$PORT

ENTRYPOINT ["dotnet", "QueueManagement.API.dll"]