FROM mcr.microsoft.com/dotnet/core/sdk:3.1.302-buster
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["out/LeakRepro"]
