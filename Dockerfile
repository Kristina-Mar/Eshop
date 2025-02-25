ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ENV ASPNETCORE_URLS=http://+:5247
EXPOSE 5247

WORKDIR /src
COPY ["./src", "."]
COPY ["./data", "data/"]
COPY ["./tests", "../tests/"]

RUN dotnet restore "./Eshop.Domain/Eshop.Domain.csproj"
RUN dotnet restore "./Eshop.KafkaBackgroundService/Eshop.KafkaBackgroundService.csproj"
RUN dotnet restore "./Eshop.Persistence/Eshop.Persistence.csproj"
RUN dotnet restore "./Eshop.WebApi/Eshop.WebApi.csproj"
RUN dotnet restore "../tests/Eshop.Test/Eshop.Test.csproj"

RUN dotnet build "./Eshop.Domain/Eshop.Domain.csproj" -c "$BUILD_CONFIGURATION" -o /app/build
RUN dotnet build "./Eshop.KafkaBackgroundService/Eshop.KafkaBackgroundService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build
RUN dotnet build "./Eshop.Persistence/Eshop.Persistence.csproj" -c "$BUILD_CONFIGURATION" -o /app/build
RUN dotnet build "./Eshop.WebApi/Eshop.WebApi.csproj" -c "$BUILD_CONFIGURATION" -o /app/build
RUN dotnet build "../tests/Eshop.Test/Eshop.Test.csproj" -c "$BUILD_CONFIGURATION" -o /app/tests

RUN dotnet publish "./Eshop.Domain/Eshop.Domain.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-x64 --self-contained false
RUN dotnet publish "./Eshop.KafkaBackgroundService/Eshop.KafkaBackgroundService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-x64 --self-contained false
RUN dotnet publish "./Eshop.Persistence/Eshop.Persistence.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-x64 --self-contained false
RUN dotnet publish "./Eshop.WebApi/Eshop.WebApi.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish -r linux-x64 --self-contained false
RUN dotnet publish "../tests/Eshop.Test/Eshop.Test.csproj" -c "$BUILD_CONFIGURATION" -o /app/tests -r linux-x64 --self-contained false

RUN dotnet test "../tests/Eshop.Test/Eshop.Test.csproj"

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run

WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /src/data ./data

RUN apt-get update && apt-get install -y sqlite3 libsqlite3-0 libsqlite3-dev \
&& rm -rf /var/lib/apt/lists/*

ENV LD_LIBRARY_PATH="/usr/lib/x86_64-linux-gnu:${LD_LIBRARY_PATH}"

ENV DB_CONNECTION_STRING="Data Source=/app/data/localdb.db"

ENTRYPOINT ["dotnet", "Eshop.WebApi.dll"]