FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm AS dev
RUN apt-get update -y && apt install -y vim pipx ruby-full build-essential zlib1g-dev

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