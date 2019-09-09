FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY KNI_D6_web.sln ./
COPY KNI_D6_web/*.csproj ./KNI_D6_web/
COPY KNI_D6_web.Model/*.csproj ./KNI_D6_web.Model/
RUN dotnet restore

COPY . ./

RUN dotnet publish ./KNI_D6_web/KNI_D6_web.csproj -c Release -o out

# Build runtime image
WORKDIR /KNI_D6_web
ENTRYPOINT ["dotnet", "KNI_D6_web.dll"]