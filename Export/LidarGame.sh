#!/bin/sh
echo -ne '\033c\033]0;LidarGame\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/LidarGame.x86_64" "$@"
