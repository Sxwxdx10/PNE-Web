#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PNE-api/PNE-api.csproj", "PNE-api/"]
COPY ["PNE-admin/PNE-admin.csproj", "PNE-admin/"]
COPY ["PNE-core/PNE-core.csproj", "PNE-core/"]
COPY ["PNE-DataAccess/PNE-DataAccess.csproj", "PNE-DataAccess/"]
RUN dotnet restore "PNE-api/PNE-api.csproj"
RUN dotnet restore "PNE-admin/PNE-admin.csproj"
COPY . .
WORKDIR "/src/PNE-api"
RUN dotnet build "./PNE-api.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR "/src/PNE-admin"
RUN dotnet build "./PNE-admin.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/PNE-api"
RUN dotnet publish "./PNE-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
WORKDIR "/src/PNE-admin"
RUN dotnet publish "./PNE-admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY ["endpointStart.sh", "app/"]
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["bash", "app/endpointStart.sh"]