FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
RUN apt-get update && \
	apt-get install -y libsnappy1v5 && \
	ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
	dotnet restore && dotnet build --no-restore && dotnet test && dotnet publish -c release -o /app --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data && \
	apt-get update && \
	apt-get install -y libsnappy1v5 && \
	ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
	rm -rf /var/lib/apt/lists/


VOLUME /data
ENV Bot__Database=/data
CMD ["./Activepass"]
