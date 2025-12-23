FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# copy only csproj
# COPY ./Support.Platform/*.csproj ./Support.Platform
# RUN dotnet restore ./Support.Platform/Support.Platform.csproj
COPY ["Support.Platform/Support.Platform.csproj", "Support.Platform/"]
RUN dotnet restore "Support.Platform/Support.Platform.csproj"

# copy from source code
# COPY Support.Platform/ Support.Platform/
COPY . .

# build and publis to out like example microsoft
WORKDIR /App/Support.Platform
RUN dotnet publish -c Release -o /App/out

# runtime image
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Support.Platform.dll"]


