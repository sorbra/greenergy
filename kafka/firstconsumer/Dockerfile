FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["firstconsumer.csproj", "./"]
RUN dotnet restore "./firstconsumer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "firstconsumer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "firstconsumer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "firstconsumer.dll"]

