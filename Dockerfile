FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
RUN apt-get update && \
	apt-get install -y libsnappy1v5 && \
	ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
	dotnet restore && dotnet build --no-restore && dotnet test && dotnet publish -c release -o /app --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data && \
	apk update && \
	apk add --no-cache snappy && \
	rm -rf /var/cache/apk/*

# ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \


VOLUME /data
ENV Bot__Database=/data
CMD ["./Activepass"]
