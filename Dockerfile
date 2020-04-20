FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
EXPOSE 80

# copy csproj and restore as distinct layers
WORKDIR /src
COPY SmartEnergy.csproj .
RUN dotnet restore

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
COPY ./entrypoint.sh /app
RUN chmod +x /app/entrypoint.sh
CMD [ "/app/entrypoint.sh" ]