FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["GCloudApp/GCloudApp.csproj", "GCloudApp/"]
RUN dotnet restore "GCloudApp/GCloudApp.csproj"
COPY . .
WORKDIR "/src/GCloudApp"
RUN dotnet build "GCloudApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GCloudApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GCloudApp.dll"]