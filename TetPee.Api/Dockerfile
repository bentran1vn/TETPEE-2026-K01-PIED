FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TetPee.Api/TetPee.Api.csproj", "TetPee.Api/"]
COPY ["TetPee.Service/TetPee.Service.csproj", "TetPee.Service/"]
COPY ["TetPee.Repository/TetPee.Repository.csproj", "TetPee.Repository/"]
RUN dotnet restore "TetPee.Api/TetPee.Api.csproj"
COPY . .
WORKDIR "/src/TetPee.Api"
RUN dotnet build "TetPee.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TetPee.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TetPee.Api.dll"]
