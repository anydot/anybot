FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
ARG RELEASE=false

WORKDIR /source

COPY . .
RUN \
    CONFIGURATION=$(if [ "$RELEASE" = "true" ] ; then echo "Release"; else echo "Debug"; fi) && \
    export CONFIGURATION && \
    echo "Build envs:" && env && echo "======" && \
    echo [*] Running tests && \
    dotnet test -c "$CONFIGURATION" /p:TreatWarningsAsErrors=true && \
    echo [*] Done && \
    dotnet publish -c "$CONFIGURATION" -o /app -r alpine-x64 --self-contained true /p:TreatWarningsAsErrors=true Anybot && \
    du -h /app

# final stage/image
FROM alpine:3.15.1

WORKDIR /app
COPY --from=build /app .

# hadolint ignore=DL3018
RUN \
    apk add --no-cache libstdc++ && \
    mkdir /data 

VOLUME /data
ENV Bot__Database=/data
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
WORKDIR /data
CMD ["../app/Anybot"]
