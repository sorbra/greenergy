
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./greenergy.api.server
COPY ./greenergy.api.server/greenergy.api.server.csproj ./greenergy.api.server
RUN dotnet restore ./greenergy.api.server

# copy everything else and build app
COPY . ./
RUN dotnet publish ./greenergy.api.server -c Release -o out

COPY ./greenergy.api.server ./greenergy.api.server/
RUN dotnet publish ./greenergy.api.server -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/greenergy.api.server/
COPY --from=build /app/greenergy.api.server/out ./
ENTRYPOINT ["dotnet", "greenergy.api.server.dll"]
