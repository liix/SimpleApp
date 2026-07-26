FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MyWebApi/MyWebApi.csproj ./MyWebApi/
RUN dotnet restore MyWebApi/MyWebApi.csproj

COPY MyWebApi/ ./MyWebApi/
RUN dotnet publish MyWebApi/MyWebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MyWebApi.dll"]