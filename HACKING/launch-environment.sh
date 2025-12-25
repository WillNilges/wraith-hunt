#!/bin/bash

set -euo pipefail

podman run --rm -it -e XDG_RUNTIME_DIR=/tmp \
           -e WAYLAND_DISPLAY=$WAYLAND_DISPLAY \
           -v $XDG_RUNTIME_DIR/$WAYLAND_DISPLAY:/tmp/$WAYLAND_DISPLAY -v ./..:/wraith-hunt devcade-dotnet
