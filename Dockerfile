FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY KNI_D6_web.sln ./
COPY KNI_D6_web/*.csproj ./KNI_D6_web/
COPY KNI_D6_web.Model/*.csproj ./KNI_D6_web.Model/
RUN dotnet restore

COPY . ./

RUN dotnet publish ./KNI_D6_web/KNI_D6_web.csproj -c Release -o /app

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "KNI_D6_web.dll"]