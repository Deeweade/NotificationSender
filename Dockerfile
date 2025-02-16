FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY NotificationSender.sln ./
COPY NotificationSender.API/NotificationSender.API.csproj NotificationSender.API/
COPY NotificationSender.Application/NotificationSender.Application.csproj NotificationSender.Application/
COPY NotificationSender.Application.Abstractions/NotificationSender.Application.Abstractions.csproj NotificationSender.Application.Abstractions/
COPY NotificationSender.Domain/NotificationSender.Domain.csproj NotificationSender.Domain/
COPY NotificationSender.Infrastructure/NotificationSender.Infrastructure.csproj NotificationSender.Infrastructure/
COPY NotificationSender.Infrastructure.Abstractions/NotificationSender.Infrastructure.Abstractions.csproj NotificationSender.Infrastructure.Abstractions/

# Восстанавливаем зависимости
RUN dotnet restore "NotificationSender.API/NotificationSender.API.csproj"

COPY . .

RUN dotnet publish "NotificationSender.API/NotificationSender.API.csproj" -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Копируем опубликованные файлы
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NotificationSender.API.dll"]