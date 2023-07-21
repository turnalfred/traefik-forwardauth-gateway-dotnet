FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ForwardAuthServer.Api/ForwardAuthServer.Api.csproj", "ForwardAuthServer.Api/"]
RUN dotnet restore "ForwardAuthServer.Api/ForwardAuthServer.Api.csproj"
COPY . .
WORKDIR "/src/ForwardAuthServer.Api"
RUN dotnet build "ForwardAuthServer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForwardAuthServer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForwardAuthServer.Api.dll"]
