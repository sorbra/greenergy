
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./emissions-api.server
COPY ./emissions-api.server/emissions-api.server.csproj ./emissions-api.server
RUN dotnet restore ./emissions-api.server

# copy everything else and build app
COPY . ./
RUN dotnet publish ./emissions-api.server -c Release -o out

COPY ./emissions-api.server ./emissions-api.server/
RUN dotnet publish ./emissions-api.server -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/emissions-api.server/
COPY --from=build /app/emissions-api.server/out ./
ENTRYPOINT ["dotnet", "emissions-api.server.dll"]
