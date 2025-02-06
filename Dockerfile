FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["NotificationSender.sln", "./"]
COPY ["NotificationSender.API/NotificationSender.API.csproj", "NotificationSender.API/"]
COPY ["NotificationSender.Application/NotificationSender.Application.csproj", "NotificationSender.Application/"]
COPY ["NotificationSender.Application.Abstractions/NotificationSender.Application.Abstractions.csproj", "NotificationSender.Application.Abstractions/"]
COPY ["NotificationSender.Domain/NotificationSender.Domain.csproj", "NotificationSender.Domain/"]
COPY ["NotificationSender.Infrastructure/NotificationSender.Infrastructure.csproj", "NotificationSender.Infrastructure/"]
COPY ["NotificationSender.Infrastructure.Abstractions/NotificationSender.Infrastructure.Abstractions.csproj", "NotificationSender.Infrastructure.Abstractions/"]

# Восстанавливаем зависимости
RUN dotnet restore "NotificationSender.API/NotificationSender.API.csproj"

# Публикуем проект в Release-режиме
RUN dotnet publish "NotificationSender.API/NotificationSender.API.csproj" -c Debug -o /app/publish

# --- Стадия исполнения ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Копируем файлы из предыдущей стадии
COPY --from=build /app/publish .

# Задаём команду запуска
ENTRYPOINT ["dotnet", "NotificationSender.API.dll"]