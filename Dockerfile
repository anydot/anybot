ARG ROCKSVER=6.11.4-r1
ARG REPOURL="https://alpine-repo.anydot.in/main"

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
ARG ROCKSVER
ARG REPOURL
ARG RELEASE=false

WORKDIR /source

COPY . .
COPY alpine-repo.anydot.in.pub /etc/apk/keys/
RUN \
    CONFIGURATION=$(if [ "$RELEASE" = "true" ] ; then echo "Release"; else echo "Debug"; fi) && \
    export CONFIGURATION && \
    echo "Build envs:" && env && echo "======" && \
    echo "$REPOURL" >> /etc/apk/repositories && \
    apk add --no-cache rocksdb=$ROCKSVER && \
    ln -s /usr/lib/librocksdb.so.6 /usr/lib/librocksdb.so && \
    echo [*] Running tests && \
    dotnet test -c "$CONFIGURATION" /p:TreatWarningsAsErrors=true && \
    echo [*] Done && \
    dotnet publish -c "$CONFIGURATION" -o /app -r alpine-x64 --self-contained true /p:TreatWarningsAsErrors=true Anybot && \
    du -h /app

# final stage/image
FROM alpine:3.13.3
ARG ROCKSVER
ARG REPOURL

WORKDIR /app
COPY --from=build /app .
COPY alpine-repo.anydot.in.pub /etc/apk/keys/
RUN \
    mkdir /data && \
    echo "$REPOURL" >> /etc/apk/repositories && \
    apk add --no-cache krb5-libs=1.18.3-r1 rocksdb=$ROCKSVER && \
    ln -s /usr/lib/librocksdb.so.6 /usr/lib/librocksdb.so

VOLUME /data
ENV Bot__Database=/data
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
WORKDIR /data
CMD ["../app/Anybot"]
