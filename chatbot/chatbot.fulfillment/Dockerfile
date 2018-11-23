
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./chatbot
RUN mkdir ./chatbot/chatbot.fulfillment
RUN mkdir ./emissions-api
RUN mkdir ./emissions-api/emissions-api.client
COPY ./chatbot/chatbot.fulfillment/chatbot.fulfillment.csproj ./chatbot/chatbot.fulfillment
COPY ./emissions-api/emissions-api.client/emissions-api.client.csproj ./emissions-api/emissions-api.client
RUN dotnet restore ./chatbot/chatbot.fulfillment

# copy everything else and build app
COPY ./chatbot/chatbot.fulfillment ./chatbot/chatbot.fulfillment/
COPY ./emissions-api/emissions-api.client ./emissions-api/emissions-api.client/
RUN dotnet publish ./chatbot/chatbot.fulfillment -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/chatbot/chatbot.fulfillment
COPY --from=build /app/chatbot/chatbot.fulfillment/out ./
ENTRYPOINT ["dotnet", "emissions-chatbot.dll"]
