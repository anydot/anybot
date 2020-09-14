ARG APORTVER=v1
ARG ROCKSVER=6.11.4-r1
ARG APORT="https://github.com/anydot/anydot.aports/releases/download/$APORTVER/packages"

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
ARG APORT
ARG ROCKSVER
ARG RELEASE=false

WORKDIR /source

COPY . .
RUN \
    CONFIGURATION=$(if [ "$RELEASE" = "true" ] ; then echo Release; else echo "Debug"; fi) && \
    export CONFIGURATION && \
    echo "Build envs:" && env && echo "======" && \
    mkdir /tmp/packages && wget -qO - $APORT | tar xC /tmp/packages && apk add --no-cache --allow-untrusted /tmp/packages/rocksdb-$ROCKSVER.apk && rm -fr /tmp/packages && \
    dotnet build -c "$CONFIGURATION" && \
    if [ "$RELEASE" != "true" ] ; then echo [*] Running tests; dotnet test -c "$CONFIGURATION"; echo [*] Done; fi && \
    dotnet publish -c "$CONFIGURATION" -o /app -r alpine-x64 -p:PublishReadyToRun=false --self-contained true -p:PublishTrimmed=true -p:SuppressTrimAnalysisWarning=false -p:TrimMode=link -p:PublishSingleFile=true -p:DebugType=none Anybot && \
    du -h /app

# final stage/image
FROM alpine:3.12
ARG APORT
ARG ROCKSVER

WORKDIR /app
COPY --from=build /app .
RUN \
    mkdir /data && \
    apk add --no-cache krb5-libs && \
    mkdir /tmp/packages && wget -qO - $APORT | tar xC /tmp/packages && apk add --no-cache --allow-untrusted /tmp/packages/rocksdb-$ROCKSVER.apk && rm -fr /tmp/packages && \
    ln -s /usr/lib/librocksdb.so.6 /app/librocksdb.so


VOLUME /data
ENV Bot__Database=/data
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
WORKDIR /data
CMD ["../app/Anybot"]
