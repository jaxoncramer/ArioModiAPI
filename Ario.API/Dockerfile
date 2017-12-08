FROM microsoft/aspnetcore-build

ADD . /src

WORKDIR /src

RUN dotnet restore

RUN dotnet publish -o /app

WORKDIR /app

ENTRYPOINT ["dotnet", "src.dll"]