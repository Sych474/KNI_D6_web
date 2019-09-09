FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY KNI_D6_web.sln ./
COPY KNI_D6_web/*.csproj ./KNI_D6_web/
COPY KNI_D6_web.Model/*.csproj ./KNI_D6_web.Model/
RUN dotnet restore

COPY . ./

RUN dotnet publish ./KNI_D6_web/KNI_D6_web.csproj -c Release -o out

WORKDIR /app/KNI_D6_web/out
CMD ["dotnet", "KNI_D6_web.dll"]
