
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

RUN mkdir ./emissions-api
RUN mkdir ./emissions-api/emissions-api.client
COPY ./emissions-api/emissions-api.client/emissions-api.client.csproj ./emissions-api/emissions-api.client

# copy csproj and restore as distinct layers
RUN mkdir ./emissions-chatbot
RUN mkdir ./emissions-chatbot/emissions-chatbot.fulfillment
COPY ./emissions-chatbot/emissions-chatbot.fulfillment/emissions-chatbot.fulfillment.csproj ./emissions-chatbot/emissions-chatbot.fulfillment
RUN dotnet restore ./emissions-chatbot/emissions-chatbot.fulfillment

# copy everything else and build app
COPY ./emissions-chatbot/emissions-chatbot.fulfillment ./emissions-chatbot/emissions-chatbot.fulfillment/
COPY ./emissions-api/emissions-api.client ./emissions-api/emissions-api.client/
RUN dotnet publish ./emissions-chatbot/emissions-chatbot.fulfillment -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/emissions-chatbot/emissions-chatbot.fulfillment
COPY --from=build /app/emissions-chatbot/emissions-chatbot.fulfillment/out ./
ENTRYPOINT ["dotnet", "emissions-chatbot.fulfillment.dll"]
