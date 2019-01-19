FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app
# copy csproj and restore as distinct layers
COPY *.sln .
COPY psytest/*.csproj ./psytest/
RUN dotnet restore

# copy everything else and build app
COPY psytest/. ./psytest/
WORKDIR /app/psytest
RUN dotnet publish -c Release -o out
COPY psytest/tests.sqlite3 /app/psytest/out
COPY psytest/users.sqlite3 /app/psytest/out
COPY psytest/results.sqlite3 /app/psytest/out


FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine AS runtime
# install libgdiplus for NPOI
RUN echo "http://dl-cdn.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories
RUN apk --update add libgdiplus
WORKDIR /app
COPY --from=build /app/psytest/out ./
CMD ASPNETCORE_URLS=http://*:$PORT dotnet psytest.dll
