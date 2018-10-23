
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app/greenergy

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/greenergy
COPY --from=build /app/greenergy/out ./
ENTRYPOINT ["dotnet", "Greenergy.dll"]
