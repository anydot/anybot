ARG SNAPPYVER=1.1.7-1

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG SNAPPYVER
ARG RELEASE=false

WORKDIR /source

COPY . .
RUN \
    CONFIGURATION=$(if [ "$RELEASE" = "true" ] ; then echo Release; else echo "Debug"; fi) && \
    export CONFIGURATION && \
    echo "Build envs:" && env && echo "======" && \
    apt-get update && \
    apt-get install -y --no-install-recommends libsnappy1v5=$SNAPPYVER && \
    ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
    dotnet build -c "$CONFIGURATION" && \
    if [ "$RELEASE" != "true" ] ; then echo [*] Running tests; dotnet test -c "$CONFIGURATION"; echo [*] Done; fi && \
    dotnet publish -c "$CONFIGURATION" -o /app -r linux-x64 -p:PublishReadyToRun=false -p:PublishTrimmed=true -p:SuppressTrimAnalysisWarning=false -p:TrimMode=link -p:PublishSingleFile=true -p:DebugType=none Anybot && \
    du -h /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
ARG SNAPPYVER

WORKDIR /app
COPY --from=build /app .
RUN \
    mkdir /data && \
    apt-get update && \
    apt-get install -y --no-install-recommends libsnappy1v5=$SNAPPYVER && \
    ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so && \
    rm -fr /var/cache/apt/archives/* /var/lib/apt/lists/*

VOLUME /data
ENV Bot__Database=/data
WORKDIR /data
CMD ["../app/Anybot"]
