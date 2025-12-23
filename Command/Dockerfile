FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# copy only csproj
# COPY ./Command/*.csproj ./Command
# RUN dotnet restore ./Command/Command.csproj
COPY ./Command.csproj ./
RUN dotnet restore 

# copy from source code
# COPY Command/ Command/
COPY . ./

# build and publis to out like example microsoft
# WORKDIR /App/Command (not using if docker one hirarki with program/root)
RUN dotnet publish -c Release -o /App/out

# runtime image
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Command.dll"]


