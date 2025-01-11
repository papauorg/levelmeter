FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm AS dev
RUN apt-get update -y && apt install -y vim pipx ruby-full build-essential zlib1g-dev libfontconfig

RUN dotnet tool install -g dotnet-format \
 && dotnet tool install -g dotnet-outdated-tool
ENV PATH="${PATH}:/home/dev/.dotnet/tools"

# Set pipx environment variables
ENV PIPX_HOME=/opt/pipx
ENV PIPX_BIN_DIR=/opt/pipxbin
ENV PATH=$PIPX_BIN_DIR:$PIPX_HOME/bin:$PATH
RUN pipx ensurepath
RUN pipx install check-jsonschema

# Set ruby gems environment variables
RUN mkdir -p /opt/gems && chown vscode:vscode /opt/gems
ENV GEM_HOME=/opt/gems
ENV PATH=$GEM_HOME/bin:$PATH

USER vscode
RUN gem install jekyll bundler

USER root


FROM dev as build
WORKDIR /build
COPY . .
RUN find ./scales/definitions/*.json | xargs -I % check-jsonschema % --schemafile ./scales/definitions/schemas/scale-config
RUN dotnet build --configuration=Release && dotnet test --no-build --configuration=Release

FROM mcr.microsoft.com/dotnet/runtime:8.0
RUN apt-get update && apt-get install -y libfontconfig
WORKDIR /app
COPY --from=build /build/src/dotnet-levelmeter/bin/Release/net8.0 .
WORKDIR /config
ENTRYPOINT [ "dotnet", "/app/dotnet-levelmeter.dll", "--"]