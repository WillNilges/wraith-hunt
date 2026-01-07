{pkgs ? import <nixpkgs> {} }:

pkgs.mkShell
{
  name = "WraithHunt";

  buildInputs = [
    pkgs.clang-tools
    pkgs.clang
  ];

  nativeBuildInputs = with pkgs; [
    cmake
    gcc
    glibc
    gdb

    # Development tools
    tiled
  ];
}
