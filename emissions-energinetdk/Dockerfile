
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./emissions-energinetdk
RUN mkdir ./emissions-api
RUN mkdir ./emissions-api/emissions-api.client
COPY ./emissions-energinetdk/emissions-energinetdk.csproj ./emissions-energinetdk
COPY ./emissions-api/emissions-api.client/emissions-api.client.csproj ./emissions-api/emissions-api.client
RUN dotnet restore ./emissions-energinetdk

# copy everything else and build app
COPY ./emissions-energinetdk ./emissions-energinetdk/
COPY ./emissions-api/emissions-api.client ./emissions-api/emissions-api.client/
RUN dotnet publish ./emissions-energinetdk -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/emissions-energinetdk/
COPY --from=build /app/emissions-energinetdk/out ./
ENTRYPOINT ["dotnet", "emissions-energinetdk.dll"]
