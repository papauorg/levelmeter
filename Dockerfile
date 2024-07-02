FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm AS dev
RUN apt-get update -y && apt install -y vim

RUN dotnet tool install -g dotnet-format \
 && dotnet tool install -g dotnet-outdated-tool
ENV PATH="${PATH}:/home/dev/.dotnet/tools" 

