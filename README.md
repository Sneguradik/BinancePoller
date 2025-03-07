# BinancePoller

## Описание
`BinancePoller` — это сервис на .NET, который запрашивает данные с Binance и отправляет их на указанные эндпоинты с заданной периодичностью.

## Функциональность
- Запрашивает свечные данные (`kline`) с Binance.
- Отправляет данные на указанный `Url`.
- Конфигурируется через JSON-файл.
- Использует `Quartz.NET` для периодического выполнения задач.
- Работает в Docker-контейнере.

## Конфигурация
Конфигурационный файл включает в себя следующие параметры:

```json
"FetchConfig": [
    {
      "Symbol" : "BTCUSDT",
      "Url" : "/indexes/v1/publish?index_code=SPBEBTC",
      "Period" : "1h"
    },
    {
      "Symbol" : "ETHUSDT",
      "Url" : "/indexes/v1/publish?index_code=SPBETH",
      "Period" : "1h"
    }
]
```

### Описание параметров:
- `Symbol` — торговая пара (например, `BTCUSDT`).
- `Url` — конечная точка для отправки данных (базовый URL зашит в `Program.cs`).
- `Period` — интервал запроса свечей (`1h`, `15m`, и т. д.).
- `LastUpdate` *(опционально)* — временная метка последнего обновления в Unix-формате.

## Расписание выполнения
Запрос совершается **раз в час**. Это настраивается в `Program.cs` через `Quartz.NET`:

```csharp
options.AddJob<BinancePollerJob>(jobKey)
    .AddTrigger(trigger => trigger.ForJob(jobKey)
    .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever()));
```

## Переменные среды
Для работы необходимо задать переменную `API_KEY`, которая используется для аутентификации Api.

## Запуск проекта

### 🔹 **Локальный запуск**

1. Установите необходимые зависимости:
```sh
dotnet restore
```
2. Запустите приложение:
```sh
dotnet run
```

### 🔹 **Запуск в Docker**

1. Соберите Docker-образ:
```sh
docker build -t binance-poller ./BinancePoller
```

2. Запустите контейнер:
```sh
docker run -e API_KEY=your_api_key -d binance-poller
```


## Развёртывание
Для деплоя можно использовать `Docker Compose`, `Kubernetes`, `systemd` или любую другую систему управления сервисами.

## Контакты
Если у вас возникли вопросы, обратитесь к разработчику или создайте issue в репозитории.