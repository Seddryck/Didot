FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

WORKDIR /app

COPY ./.publish/net8.0/linux-x64 ./

ENTRYPOINT ["./Didot"]
