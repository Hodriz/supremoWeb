FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
#ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
#EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SupremoWeb/SupremoWeb.csproj", "SupremoWeb/"]
RUN dotnet restore "SupremoWeb/SupremoWeb.csproj"
COPY . .
WORKDIR "/src/SupremoWeb"
RUN dotnet build "SupremoWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupremoWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chmod -R 755 /app/wwwroot
ENTRYPOINT ["dotnet", "SupremoWeb.dll"]