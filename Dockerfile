FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data
VOLUME /data
ENV Bot__Database=/data
CMD ["./Activepass"]