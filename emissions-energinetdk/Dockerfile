
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./emissions-energinetdk
COPY ./emissions-energinetdk/emissions-energinetdk.csproj ./emissions-energinetdk
RUN dotnet restore ./emissions-energinetdk

# copy everything else and build app
COPY ./emissions-energinetdk ./emissions-energinetdk/
RUN dotnet publish ./emissions-energinetdk -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/emissions-energinetdk/
COPY --from=build /app/emissions-energinetdk/out ./
ENTRYPOINT ["dotnet", "emissions-energinetdk.dll"]
