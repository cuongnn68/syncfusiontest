# ARG DOTNET_VERSION
# FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy
# ENV DOTNET_RUNNING_IN_CONTAINER=true
# WORKDIR /app
# COPY ./release .
# EXPOSE 80
# ENTRYPOINT ["dotnet", "syncfusiontest.dll"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# word => pdf
# https://help.syncfusion.com/file-formats/docio/faq#how-to-install-necessary-fonts-in-linux-containers
# RUN apt-get update -y && apt-get install fontconfig -y 
RUN apt-get update -y && apt-get install libfontconfig -y
RUN echo "deb http://httpredir.debian.org/debian buster main contrib non-free" > /etc/apt/sources.list \ 
    && echo "deb http://httpredir.debian.org/debian buster-updates main contrib non- free" >> /etc/apt/sources.list \
    && echo "deb http://security.debian.org/ buster/updates main contrib non-free" >> /etc/apt/sources.list \
    && echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections \
    && apt-get update \
    && apt-get install -y \
        fonts-arphic-ukai \
        fonts-arphic-uming \
        fonts-ipafont-mincho \
        fonts-ipafont-gothic \
        fonts-unfonts-core \
        ttf-wqy-zenhei \
        ttf-mscorefonts-installer \
    && apt-get clean \
    && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "syncfusiontest.dll"]