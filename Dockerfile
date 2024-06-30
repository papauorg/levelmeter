FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm AS dev

RUN apt-get update -y && \
    apt-get install -y libgdiplus



