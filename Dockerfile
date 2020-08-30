FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY . .
RUN apt-get update && \
    apt-get install -y libsnappy1v5 && \
    ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
    dotnet restore && dotnet build --no-restore && dotnet test && dotnet publish -c release -o /app --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data && \
    apt-get update && \
    apt-get install -y libsnappy1v5 && \
    ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
    rm -fr /var/cache/apt/archives/*

VOLUME /data
ENV Bot__Database=/data
WORKDIR /data
CMD ["../app/Anybot"]
