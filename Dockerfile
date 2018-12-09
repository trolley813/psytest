FROM microsoft/dotnet:2.1-sdk

WORKDIR /app

COPY . .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet psytest.dll
