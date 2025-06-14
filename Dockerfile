FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine@sha256:a968901b80b42c4d1acd33ee5c27be1d2d3d7e9aa3d6b80d16663889899ba9f3 AS build
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
    dotnet publish -c "$CONFIGURATION" -o /app -r linux-musl-x64 --self-contained true /p:TreatWarningsAsErrors=true Anybot && \
    du -h /app

# final stage/image
FROM alpine:3.22.0

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
