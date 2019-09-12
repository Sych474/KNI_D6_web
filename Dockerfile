FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY KNI_D6_web.sln ./
COPY KNI_D6_web/*.csproj ./KNI_D6_web/
COPY KNI_D6_web.Model/*.csproj ./KNI_D6_web.Model/
RUN dotnet restore

COPY . ./
WORKDIR /app/KNI_D6_web
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/KNI_D6_web/out ./
ENTRYPOINT ["dotnet", "KNI_D6_web.dll"]
