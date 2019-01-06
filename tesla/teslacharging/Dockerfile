
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./teslacharging
COPY ./teslacharging/teslacharging.csproj ./teslacharging
RUN dotnet restore ./teslacharging

# copy everything else and build app
COPY . ./
RUN dotnet publish ./teslacharging -c Release -o out

COPY ./teslacharging ./teslacharging/
RUN dotnet publish ./teslacharging -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/teslacharging/
COPY --from=build /app/teslacharging/out ./
ENTRYPOINT ["dotnet", "teslacharging.dll"]
