
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app/greenergy.syncdata.energinet-dk

# copy csproj and restore as distinct layers
COPY *.csproj ./
COPY ../greenergy.datalayer/*.csproj ../greenergy.datalayer
RUN dotnet restore

# copy everything else and build app
COPY . ./
COPY ../greenergy.datalayer ../greenergy.datalayer
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/greenergy.syncdata.energinet-dk
COPY --from=build /app/greenergy.syncdata.energinet-dk/out ./
ENTRYPOINT ["dotnet", "greenergy.syncdata.energinet-dk.dll"]
