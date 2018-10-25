
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir ./greenergy.chatbot
RUN mkdir ./greenergy.api.client
COPY ./greenergy.chatbot/greenergy.chatbot.csproj ./greenergy.chatbot
COPY ./greenergy.api.client/greenergy.api.client.csproj ./greenergy.api.client
RUN dotnet restore ./greenergy.chatbot

# copy everything else and build app
COPY ./greenergy.chatbot ./greenergy.chatbot/
COPY ./greenergy.api.client ./greenergy.api.client/
RUN dotnet publish ./greenergy.chatbot -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app/greenergy.chatbot/
COPY --from=build /app/greenergy.chatbot/out ./
ENTRYPOINT ["dotnet", "greenergy.chatbot.dll"]
