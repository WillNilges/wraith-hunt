FROM debian:trixie

RUN apt-get -y update
RUN apt-get -y install vim wget git fonts-liberation fontconfig-config tmux xterm pulseaudio
RUN wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb \
     -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0 libgtk-3-0

RUN apt-get install -y cmake pkg-config mesa-utils libglu1-mesa-dev freeglut3-dev mesa-common-dev libglew-dev libglfw3-dev libglm-dev libao-dev libmpg123-dev

RUN dotnet tool install -g dotnet-mgcb; dotnet tool restore
