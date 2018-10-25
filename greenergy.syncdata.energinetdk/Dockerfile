
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./greenergy.syncdata.energinetdk
RUN mkdir ./greenergy.api.client
COPY ./greenergy.syncdata.energinetdk/greenergy.syncdata.energinetdk.csproj ./greenergy.syncdata.energinetdk
COPY ./greenergy.api.client/greenergy.api.client.csproj ./greenergy.api.client
RUN dotnet restore ./greenergy.syncdata.energinetdk

# copy everything else and build app
COPY ./greenergy.syncdata.energinetdk ./greenergy.syncdata.energinetdk/
COPY ./greenergy.api.client ./greenergy.api.client/
RUN dotnet publish ./greenergy.syncdata.energinetdk -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/greenergy.syncdata.energinetdk/
COPY --from=build /app/greenergy.syncdata.energinetdk/out ./
ENTRYPOINT ["dotnet", "greenergy.syncdata.energinetdk.dll"]
