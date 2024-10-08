#!/bin/bash
#
# SPDX-FileCopyrightText: 2023 Zextras <https://www.zextras.com>
#
# SPDX-License-Identifier: GPL-2.0-only
#

# Check if the correct number of arguments is provided
if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <DISTRO>"
  echo "Available DISTROs: ubuntu, rocky"
  exit 1
fi

DISTRO=$1
YAP_FLAGS="-c"
YAP_VERSION=1.11

# Validate the DISTRO input
case $DISTRO in
  ubuntu)
    echo "Building for DISTRO: $DISTRO"
    DISTRO+=-jammy
    ;;
  rocky)
    echo "Building for DISTRO: $DISTRO"
    DISTRO+=-8
    ;;
  *)
    echo "Invalid DISTRO: $DISTRO"
    echo "Available DISTROs: ubuntu, rocky"
    exit 1
    ;;
esac

docker run -ti \
  --workdir /project \
  -v "$(pwd):/project" \
  "docker.io/m0rf30/yap-$DISTRO:$YAP_VERSION" \
  build \
  "$DISTRO" \
  /project/package \
  $YAP_FLAGS
