# Использование
## Первоначальная настройка перед запуском тестов:
Установка браузеров:
```shell
pwsh bin/Debug/netX/playwright.ps1 install  
```

Перед запуском установить переменную для выполнения теста в режиме headed (в режиме headless все тесты падают на моменте загрузки страницы, потому что сайт ловит автоматические запросы):
```shell
 $env:HEADED=1
```

Запустить все тесты из консоли:
```shell
dotnet test
```

Чтобы заупстить тесты отдельно:
```shell
dotnet test --filter TestCountAndRegion
dotnet test --filter TestPopupAndCart
dotnet test --filter TestTitlesAndPrices
dotnet test --filter TestСontacts
```

## Для того, чтобы собрать отчет в Allure:
```shell
dotnet test
```

Затем перейти в директорию:
```shell
cd .\\bin\\Debug\\net7.0\\
```

Для генерации отчета:
```shell
allure serve
```